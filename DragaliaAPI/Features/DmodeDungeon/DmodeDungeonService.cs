using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.DmodeDungeon;

public class DmodeDungeonService(
    IDmodeRepository dmodeRepository,
    IDmodeCacheService dmodeCacheService,
    IUnitRepository unitRepository,
    IDmodeService dmodeService,
    ILogger<DmodeDungeonService> logger,
    IRewardService rewardService
) : IDmodeDungeonService
{
    private const int MaxFloor = 60;
    private readonly Random rdm = Random.Shared;

    public async Task<(DungeonState State, DmodeIngameData IngameData)> StartDungeon(
        Charas charaId,
        int startFloor,
        int servitorId,
        IEnumerable<Charas> editSkillCharaIds
    )
    {
        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();
        DbPlayerDmodeInfo info = await dmodeRepository.GetInfoAsync();

        DbPlayerDmodeChara dmodeChara =
            await dmodeRepository.Charas.SingleOrDefaultAsync(x => x.CharaId == charaId)
            ?? dmodeRepository.AddChara(charaId);

        dmodeChara.SelectEditSkillCharaId1 = editSkillCharaIds.ElementAtOrDefault(0);
        dmodeChara.SelectEditSkillCharaId2 = editSkillCharaIds.ElementAtOrDefault(1);
        dmodeChara.SelectedServitorId = servitorId;

        dungeon.State = startFloor == 1 ? DungeonState.WaitingInitEnd : DungeonState.WaitingSkip;

        dungeon.Floor = startFloor;
        dungeon.CharaId = charaId;
        dungeon.DungeonScore = 0;
        dungeon.IsPlayEnd = false;
        dungeon.QuestTime = 0;

        DbPlayerCharaData chara = await unitRepository.Charas.SingleAsync(
            x => x.CharaId == charaId
        );

        CharaData charaData = MasterAsset.CharaData[charaId];

        AtgenUnitData unitData =
            new(
                chara.CharaId,
                charaData.HasManaSpiral ? 4 : 3,
                charaData.HasManaSpiral ? 3 : 2,
                charaData.MaxAbility1Level,
                charaData.MaxAbility2Level,
                charaData.MaxAbility3Level,
                5,
                5,
                2,
                charaData.HasManaSpiral ? 1 : 0
            );

        const int dmodeLevelGroupId = 1; // Everyone has 1 /* MasterAsset.CharaData[chara.CharaId].DmodeLevelGroupId; */

        DmodeIngameData ingame =
            new(
                string.Empty,
                startFloor,
                MaxFloor,
                info.RecoveryCount,
                info.RecoveryTime,
                servitorId,
                dmodeLevelGroupId,
                unitData,
                await dmodeService.GetServitorPassiveList()
            );

        // sets unique_key
        await dmodeCacheService.StoreIngameInfo(ingame);

        // We have to generate the first floor here. Why?
        // Because the edit skills are converted to dungeon items which get stored in DmodeFloorData.dmode_dungeon_odds.
        // As this is the only place where we have access to them (without explicitly storing them), we just generate the floor and a fake play record here.

        DmodeFloorData firstFloor = GenerateFirstFloor(ingame, charaId, editSkillCharaIds);

        await dmodeCacheService.StoreFloorInfo(firstFloor);

        DmodePlayRecord firstPlayRecord =
            new() { unique_key = ingame.unique_key, floor_key = firstFloor.floor_key };

        await dmodeCacheService.StorePlayRecord(firstPlayRecord);

        return (dungeon.State, ingame);
    }

    public async Task<(DungeonState State, DmodeIngameData IngameData)> RestartDungeon()
    {
        logger.LogDebug("Restarting dmode floor");

        await dmodeService.UseRecovery();

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();

        dungeon.State = DungeonState.RestartEnd;

        DmodeIngameData ingame = await dmodeCacheService.LoadIngameInfo();

        DbPlayerDmodeInfo info = await dmodeRepository.GetInfoAsync();

        ingame.recovery_count = info.RecoveryCount;
        ingame.recovery_time = info.RecoveryTime;

        await dmodeCacheService.StoreIngameInfo(ingame);

        return (dungeon.State, ingame);
    }

    // TODO: Add random equipment
    public async Task<DungeonState> SkipFloor()
    {
        logger.LogDebug("Skipping dmode floors");

        await dmodeService.UseSkip();

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();

        dungeon.State = DungeonState.WaitingSkipEnd;

        return dungeon.State;
    }

    public async Task<DungeonState> HaltDungeon(bool userHalt)
    {
        logger.LogDebug("Halting dungeon. User halt: {isUserHalt}", userHalt);

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();

        dungeon.State = DungeonState.Halting;

        return dungeon.State;
    }

    public async Task<(DungeonState State, DmodeIngameResult IngameResult)> FinishDungeon(
        bool isGameOver
    )
    {
        DmodeIngameData ingameData = await dmodeCacheService.LoadIngameInfo();
        DmodePlayRecord playRecord = await dmodeCacheService.LoadPlayRecord();
        DmodeFloorData floorData = await dmodeCacheService.LoadFloorInfo(playRecord.floor_key);

        int floorNum = playRecord.is_floor_incomplete
            ? playRecord.floor_num - 1
            : playRecord.floor_num; // We take the floor num from playRecord due to the additional /floor call on pressing end

        Charas charaId = ingameData.unit_data.chara_id;

        List<AtgenRewardTalismanList> talismans = new();

        if (!isGameOver)
        {
            int talismanAmount = floorNum switch
            {
                < 50 => rdm.Next(1, 2),
                < 60 => rdm.Next(3, 4),
                _ => 6
            };

            for (int i = 0; i < talismanAmount; i++)
            {
                AtgenRewardTalismanList talisman = TalismanHelper.GenerateTalisman(
                    rdm,
                    charaId,
                    floorNum
                );

                await rewardService.GrantTalisman(
                    talisman.talisman_id,
                    talisman.talisman_ability_id_1,
                    talisman.talisman_ability_id_2,
                    talisman.talisman_ability_id_3,
                    talisman.additional_hp,
                    talisman.additional_attack
                );

                talismans.Add(talisman);
            }
        }

        Dictionary<DmodeServitorPassiveType, int> passives =
            ingameData.dmode_servitor_passive_list.ToDictionary(
                x => x.passive_no,
                x => x.passive_level
            );

        double pointMultiplier1 = isGameOver ? 0.5d : 1d;
        double pointMultiplier2 = isGameOver ? 0.5d : 1d;

        if (passives.TryGetValue(DmodeServitorPassiveType.DmodePoint1, out int dmodePoint1Level))
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.DmodePoint1
            ][dmodePoint1Level];
            pointMultiplier1 += level.UpValue / 100;
        }

        if (passives.TryGetValue(DmodeServitorPassiveType.DmodePoint2, out int dmodePoint2Level))
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.DmodePoint2
            ][dmodePoint2Level];
            pointMultiplier2 += level.UpValue / 100;
        }

        DmodeIngameResult ingameResult =
            new()
            {
                floor_num = floorNum,
                is_record_floor_num = floorNum > await dmodeRepository.GetTotalMaxFloorAsync(),
                chara_id_list = new List<Charas> { ingameData.unit_data.chara_id, },
                quest_time = floorData.dmode_area_info.quest_time,
                is_view_quest_time = ingameData.start_floor_num == 1,
                dmode_score = floorData.dmode_area_info.dmode_score,
                reward_talisman_list = talismans,
                take_dmode_point_1 = (int)
                    Math.Ceiling(floorData.dmode_unit_info.take_dmode_point_1 * pointMultiplier1),
                take_dmode_point_2 = (int)
                    Math.Ceiling(floorData.dmode_unit_info.take_dmode_point_2 * pointMultiplier2),
                take_player_exp = 0,
                player_level_up_fstone = 0,
                clear_state = isGameOver ? 0 : 1
            };

        DbPlayerDmodeChara chara = await dmodeRepository.Charas.SingleAsync(
            x => x.CharaId == charaId
        );

        if (ingameResult.floor_num > chara.MaxFloor)
            chara.MaxFloor = ingameResult.floor_num;

        if (ingameResult.dmode_score > chara.MaxScore)
            chara.MaxScore = ingameResult.dmode_score;

        await rewardService.GrantReward(
            new Entity(
                EntityTypes.DmodePoint,
                (int)DmodePoint.Point1,
                ingameResult.take_dmode_point_1
            )
        );
        await rewardService.GrantReward(
            new Entity(
                EntityTypes.DmodePoint,
                (int)DmodePoint.Point2,
                ingameResult.take_dmode_point_2
            )
        );

        // Clear cache
        await dmodeCacheService.DeletePlayRecord();
        await dmodeCacheService.DeleteFloorInfo(playRecord.floor_key);
        await dmodeCacheService.DeleteIngameInfo();

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();
        dungeon.Clear();

        return (dungeon.State, ingameResult);
    }

    public async Task<(DungeonState State, DmodeFloorData FloorData)> ProgressToNextFloor(
        DmodePlayRecord? playRecord
    )
    {
        DmodeIngameData ingameData = await dmodeCacheService.LoadIngameInfo();

        DmodeFloorData floorData;

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();

        if (playRecord == null)
        {
            if (await dmodeCacheService.DoesPlayRecordExist())
            {
                // Used for restarting runs and the first floor (see dungeon/start for an explanation)
                playRecord = await dmodeCacheService.LoadPlayRecord();
                floorData = await dmodeCacheService.LoadFloorInfo(playRecord.floor_key);
            }
            else
            {
                Debugger.Break();
                // The first floor should have been generated by dungeon/start, so this should never happen
                throw new DragaliaException(ResultCode.DmodeDungeonCommonError, "Floor not found");
            }
        }
        else
        {
            floorData = await GenerateSubsequentFloor(playRecord, ingameData);
            await dmodeCacheService.StoreFloorInfo(floorData);
        }

        dungeon.State = DungeonState.Playing;
        dungeon.Floor = floorData.dmode_area_info.floor_num;
        dungeon.DungeonScore = floorData.dmode_area_info.dmode_score;
        dungeon.QuestTime = (int)Math.Ceiling(floorData.dmode_area_info.quest_time);
        dungeon.IsPlayEnd = floorData.is_play_end;

        logger.LogDebug(
            "Generated dmode floor with area info {@dmodeAreaInfo}",
            floorData.dmode_area_info
        );

        return (dungeon.State, floorData);
    }

    private DmodeFloorData GenerateFirstFloor(
        DmodeIngameData ingameData,
        Charas charaId,
        IEnumerable<Charas> editSkillCharaIds
    )
    {
        DmodeFloorData floorData =
            new()
            {
                unique_key = ingameData.unique_key,
                floor_key = string.Empty, // Will be set by cache service
                is_end = ingameData.start_floor_num == ingameData.target_floor_num,
                is_play_end = false,
                is_view_area_start_equipment = ingameData.start_floor_num != 1, // For the first floor of skip runs
                dmode_area_info = null,
                dmode_unit_info = null,
                dmode_dungeon_odds = null
            };

        DmodeQuestFloor floor = MasterAsset.DmodeQuestFloor[ingameData.start_floor_num];

        List<DmodeDungeonItemList> editSkillItemList = new();

        foreach (Charas editSkillCharaId in editSkillCharaIds)
        {
            if (editSkillCharaId == 0)
                continue;

            CharaData chara = MasterAsset.CharaData[editSkillCharaId];
            editSkillItemList.Add(
                new DmodeDungeonItemList(
                    editSkillItemList.Count + 1,
                    chara.EditSkillId,
                    DmodeDungeonItemState.SkillBag,
                    new AtgenOption()
                )
            );
        }

        floorData.dmode_area_info = GenerateAreaInfo(floor, 0, 0, floorData.floor_key);
        floorData.dmode_unit_info = new AtgenDmodeUnitInfo
        {
            level = 1,
            exp = 0,
            equip_crest_item_no_sort_list = new int[3],
            bag_item_no_sort_list = new int[10],
            skill_bag_item_no_sort_list = new int[8],
            dmode_hold_dragon_list = Enumerable.Empty<AtgenDmodeHoldDragonList>(),
            take_dmode_point_1 = 0,
            take_dmode_point_2 = 0
        };

        floorData.dmode_dungeon_odds = GenerateOddsInfo(
            floor,
            floorData.dmode_area_info,
            editSkillItemList,
            floorData.dmode_unit_info,
            ingameData.unit_data.chara_id
        );

        if (floor.FloorNum > 1)
        {
            int newLevel = floor.BaseEnemyLevel + 5;

            floorData.dmode_unit_info.level = newLevel;
            floorData.dmode_unit_info.exp = MasterAsset.DmodeCharaLevel[1000 + newLevel].TotalExp;

            List<AtgenDmodeHoldDragonList> holdDragonList = new();

            int minRarity = GetMinRarity(floor.FloorNum);

            DmodeDungeonItemData[] dragonPool = MasterAsset.DmodeDungeonItemData.Enumerable
                .Where(x => x.DmodeDungeonItemType == DmodeDungeonItemType.Dragon)
                .Where(
                    x =>
                        floorData.dmode_dungeon_odds.dmode_select_dragon_list.All(
                            y => y.dragon_id != (Dragons)x.Id
                        )
                )
                .ToArray();

            for (int i = 0; i < floor.FloorNum; i += 10)
            {
                DmodeDungeonItemData dragon;
                do
                {
                    dragon = rdm.Next(dragonPool);
                } while (holdDragonList.Any(x => (int)x.dragon_id == dragon.Id));

                holdDragonList.Add(
                    new AtgenDmodeHoldDragonList() { count = 0, dragon_id = (Dragons)dragon.Id }
                );
            }

            floorData.dmode_unit_info.dmode_hold_dragon_list = holdDragonList;

            int slot = 0;
            for (int i = 0; i < floor.FloorNum; i += 20)
            {
                DmodeDungeonItemList crest = GenerateDungeonAbilityCrest(minRarity + 1);
                crest.item_state = DmodeDungeonItemState.EquipCrest;

                floorData.dmode_dungeon_odds.dmode_dungeon_item_list.AddDmodeItem(crest);
                floorData.dmode_unit_info.equip_crest_item_no_sort_list[slot] = crest.item_no;
                slot++;
            }

            WeaponTypes weaponType = MasterAsset.CharaData[charaId].WeaponType;
            DmodeDungeonItemList weapon = GenerateDungeonWeapon(minRarity + 1, weaponType);
            weapon.item_state = DmodeDungeonItemState.EquipWeapon;

            floorData.dmode_dungeon_odds.dmode_dungeon_item_list.AddDmodeItem(weapon);
        }

        return floorData;
    }

    private async Task<DmodeFloorData> GenerateSubsequentFloor(
        DmodePlayRecord playRecord,
        DmodeIngameData ingameData
    )
    {
        await dmodeCacheService.StorePlayRecord(playRecord);

        DmodeFloorData previousFloor = await dmodeCacheService.LoadFloorInfo(playRecord.floor_key);

        AtgenDmodeUnitInfo unitInfo = previousFloor.dmode_unit_info;

        int dmodeScore = previousFloor.dmode_area_info.dmode_score;

        AtgenDmodeEnemy[] enemies =
            previousFloor.dmode_dungeon_odds.dmode_odds_info.dmode_enemy.ToArray();
        int[] enemyKilledStatus = playRecord.dmode_treasure_record.enemy.ToArray();

        double expMultiplier = GetExpMultiplier(ingameData);

        // First, process enemy kills and rewards
        for (int i = 0; i < Math.Min(enemyKilledStatus.Length, enemies.Length); i++)
        {
            if (enemyKilledStatus[i] != 1)
                continue;

            AtgenDmodeEnemy enemy = enemies[i];
            int dmodeEnemyParamGroupId = MasterAsset.EnemyParam[
                enemy.param_id
            ].DmodeEnemyParamGroupId;

            if (
                !MasterAsset.DmodeEnemyParam.TryGetValue(
                    (dmodeEnemyParamGroupId * 1000) + Math.Min(enemy.level, 100),
                    out DmodeEnemyParam? enemyParam
                )
            )
            {
                continue;
            }
            unitInfo.exp += (int)Math.Ceiling(enemyParam.DropExp * expMultiplier);
            unitInfo.take_dmode_point_1 += enemyParam.DropDmodePoint1;
            unitInfo.take_dmode_point_2 += enemyParam.DropDmodePoint2;
            dmodeScore += enemyParam.DmodeScore;
        }

        // Now process levels
        DmodeCharaLevel currentLevel = MasterAsset.DmodeCharaLevel[1000 + unitInfo.level]; // DmodeLevelGroup * 1000 (always 1) + level
        while (
            currentLevel.NecessaryExp != 0
            && unitInfo.exp > currentLevel.TotalExp + currentLevel.NecessaryExp
        )
        {
            unitInfo.level++;
            currentLevel = MasterAsset.DmodeCharaLevel[1000 + unitInfo.level];
        }

        // Now process item list changes
        Dictionary<int, DmodeDungeonItemList> itemList =
            previousFloor.dmode_dungeon_odds.dmode_dungeon_item_list.ToDictionary(
                x => x.item_no,
                x => x
            );

        foreach (
            AtgenDmodeDungeonItemStateList stateChange in playRecord.dmode_dungeon_item_state_list
        )
        {
            if (stateChange.state == DmodeDungeonItemState.Sell)
            {
                DmodeDungeonItemData item = MasterAsset.DmodeDungeonItemData[
                    itemList[stateChange.item_no].item_id
                ];
                unitInfo.take_dmode_point_1 += item.SellDmodePoint1;
                unitInfo.take_dmode_point_2 += item.SellDmodePoint2;
                itemList[stateChange.item_no].item_state = DmodeDungeonItemState.None;
            }
            else
            {
                itemList[stateChange.item_no].item_state = stateChange.state;
            }
        }

        foreach (
            AtgenDmodeDungeonItemOptionList optionChange in playRecord.dmode_dungeon_item_option_list
        )
        {
            itemList[optionChange.item_no].option.abnormal_status_invalid_count =
                optionChange.abnormal_status_invalid_count;
        }

        Dictionary<Dragons, int> dragonDict = unitInfo.dmode_hold_dragon_list.ToDictionary(
            x => x.dragon_id,
            x => x.count
        );

        foreach (AtgenDmodeDragonUseList dragonUse in playRecord.dmode_dragon_use_list)
        {
            dragonDict[dragonUse.dragon_id] += dragonUse.use_count;
        }

        // Now process dragon selection (if applicable)
        if (playRecord.select_dragon_no != 0)
        {
            List<AtgenDmodeSelectDragonList> dragonSelection =
                previousFloor.dmode_dungeon_odds.dmode_select_dragon_list.ToList();
            AtgenDmodeSelectDragonList dragon = dragonSelection[playRecord.select_dragon_no - 1];
            if (dragon.is_rare)
            {
                if (
                    dragon.pay_dmode_point_1 != 0
                    && dragon.pay_dmode_point_1 > unitInfo.take_dmode_point_1
                )
                {
                    throw new DragaliaException(
                        ResultCode.DmodeDungeonFloorDragonParamInconsistent,
                        "Not enough points"
                    );
                }

                unitInfo.take_dmode_point_1 -= dragon.pay_dmode_point_1;

                if (
                    dragon.pay_dmode_point_2 != 0
                    && dragon.pay_dmode_point_2 > unitInfo.take_dmode_point_2
                )
                {
                    throw new DragaliaException(
                        ResultCode.DmodeDungeonFloorDragonParamInconsistent,
                        "Not enough points"
                    );
                }

                unitInfo.take_dmode_point_2 -= dragon.pay_dmode_point_2;
            }

            dragonDict[dragon.dragon_id] = 0;
        }

        // Transfer properties from play record
        unitInfo.bag_item_no_sort_list = playRecord.bag_item_no_sort_list;
        unitInfo.equip_crest_item_no_sort_list = playRecord.equip_crest_item_no_sort_list;
        unitInfo.skill_bag_item_no_sort_list = playRecord.skill_bag_item_no_sort_list;
        unitInfo.dmode_hold_dragon_list = dragonDict.Select(
            x => new AtgenDmodeHoldDragonList(x.Key, x.Value)
        );

        // Generate random floor data
        DmodeQuestFloor floor = MasterAsset.DmodeQuestFloor[Math.Min(playRecord.floor_num + 1, 60)];

        AtgenDmodeAreaInfo areaInfo = GenerateAreaInfo(
            floor,
            playRecord.quest_time,
            dmodeScore,
            previousFloor.floor_key
        );
        AtgenDmodeDungeonOdds odds = GenerateOddsInfo(
            floor,
            areaInfo,
            itemList.Values.ToList(),
            unitInfo,
            ingameData.unit_data.chara_id
        );

        DmodeFloorData floorData =
            new()
            {
                unique_key = previousFloor.unique_key,
                floor_key = previousFloor.floor_key, // Done so we can always reference the current floor
                is_end = playRecord.floor_num == ingameData.target_floor_num, // Game ignores this
                is_play_end = playRecord.is_floor_incomplete, // Game ignores this (it only checks the one in DmodeIngameData)
                is_view_area_start_equipment = false, // This can never be true as it only applies to the first floor after skipping
                dmode_area_info = areaInfo,
                dmode_unit_info = unitInfo,
                dmode_dungeon_odds = odds
            };

        return floorData;
    }

    private AtgenDmodeAreaInfo GenerateAreaInfo(
        DmodeQuestFloor floor,
        float questTime,
        int score,
        string floorKey
    )
    {
        int floorTheme = 0;

        while (floorTheme == 0)
        {
            floorTheme = rdm.Next(floor.AvailableThemes);
        }

        List<DmodeDungeonArea> areas = MasterAsset.DmodeDungeonArea.Enumerable
            .Where(x => x.ThemeGroupId == floorTheme)
            .ToList();

        DmodeDungeonArea area = floor.Id is 45 or 50 // Agito bosses
            ? areas[floorKey.Sum(x => x) % areas.Count] // NOTE: This needs to be changed if floor key is made floor-independent
            : rdm.Next(areas);

        return new AtgenDmodeAreaInfo(floor.FloorNum, questTime, score, area.ThemeGroupId, area.Id);
    }

    private AtgenDmodeDungeonOdds GenerateOddsInfo(
        DmodeQuestFloor floor,
        AtgenDmodeAreaInfo dmodeAreaInfo,
        List<DmodeDungeonItemList>? existingItems,
        AtgenDmodeUnitInfo dmodeUnitInfo,
        Charas charaId
    )
    {
        DmodeDungeonTheme theme = MasterAsset.DmodeDungeonTheme[
            dmodeAreaInfo.current_area_theme_id
        ];

        DmodeDungeonArea area = MasterAsset.DmodeDungeonArea[dmodeAreaInfo.current_area_id];
        string assetName = $"{area.Scene}/{area.AreaName}".ToLowerInvariant();

        DmodeAreaInfo areaInfo = MasterAsset.DmodeAreaInfo[assetName];

        // Now the actual generation

        List<DmodeDungeonItemList> itemList = existingItems ?? new();
        WeaponTypes weaponType = MasterAsset.CharaData[charaId].WeaponType;

        IEnumerable<AtgenDmodeEnemy> dmodeEnemies = GenerateEnemies(
            areaInfo,
            area.IsSelectedEntity,
            floor.BaseEnemyLevel,
            floor.BaseBossEnemyLevel,
            theme.PlusLevelMin,
            theme.PlusLevelMax,
            toughness =>
            {
                int rarityPool = rdm.Next(100);
                rarityPool += toughness * 10;

                int rarity = rarityPool switch
                {
                    > 98 => 5,
                    > 93 => 4,
                    > 85 => 3,
                    > 75 => 2,
                    _ => 1
                };

                rarity = ClampRarityByFloor(dmodeAreaInfo.floor_num, rarity);

                (DmodeDungeonItemList item, DmodeDungeonItemType type) = GenerateDungeonItem(
                    rarity,
                    weaponType,
                    itemList
                );

                return new AtgenDmodeDropList(EntityTypes.DmodeDungeonItem, item.item_no, 1);
            }
        );

        IEnumerable<AtgenDmodeDropObj> dmodeDropObjs = GenerateDrops(
            areaInfo,
            remainingPool =>
            {
                int rarityPool = rdm.Next(100);
                rarityPool += dmodeAreaInfo.floor_num;

                int rarity = rarityPool switch
                {
                    > 98 when remainingPool >= 3 => 5,
                    > 93 when remainingPool >= 2 => 4,
                    > 85 when remainingPool >= 2 => 3,
                    > 75 when remainingPool >= 2 => 2,
                    _ => 1
                };

                rarity = ClampRarityByFloor(dmodeAreaInfo.floor_num, rarity);

                (DmodeDungeonItemList item, DmodeDungeonItemType type) = GenerateDungeonItem(
                    rarity,
                    weaponType,
                    itemList
                );

                return (
                    new AtgenDmodeDropList(EntityTypes.DmodeDungeonItem, item.item_no, 1),
                    type,
                    rarity
                );
            }
        );

        DmodeOddsInfo oddsInfo = new(dmodeDropObjs, dmodeEnemies);

        List<AtgenDmodeSelectDragonList> dragonList = new();

        if (theme.BossAppear && dmodeUnitInfo.dmode_hold_dragon_list.Count() < 8)
        {
            List<int> alreadyRolledDragonIds = new();
            IEnumerable<int> alreadyOwnedDragonIds = dmodeUnitInfo.dmode_hold_dragon_list.Select(
                x => (int)x.dragon_id
            );

            DmodeDungeonItemData[] dragonPool = MasterAsset.DmodeDungeonItemData.Enumerable
                .Where(x => x.DmodeDungeonItemType == DmodeDungeonItemType.Dragon)
                .ExceptBy(alreadyOwnedDragonIds, x => x.Id)
                .ToArray();

            for (int i = 0; i < 3; i++)
            {
                DmodeDungeonItemData dragon;

                do
                {
                    dragon = rdm.Next(dragonPool);
                } while (alreadyRolledDragonIds.Contains(dragon.Id));

                alreadyRolledDragonIds.Add(dragon.Id);

                AtgenDmodeSelectDragonList selectDragon = new();

                selectDragon.select_dragon_no = i + 1;
                selectDragon.dragon_id = (Dragons)dragon.DungeonItemTargetId;

                int dragonRarity = MasterAsset.DragonData[selectDragon.dragon_id].Rarity;

                if (rdm.Next(100) > 80 && dragonList.Any(x => !x.is_rare))
                {
                    selectDragon.is_rare = true;
                    selectDragon.pay_dmode_point_1 = rdm.Next(
                        (int)Math.Ceiling(floor.Id * 0.5d),
                        (int)Math.Ceiling(floor.Id * 2d)
                    );
                    selectDragon.pay_dmode_point_2 = 0;
                }

                dragonList.Add(selectDragon);
            }
        }

        return new AtgenDmodeDungeonOdds(dragonList, itemList, oddsInfo);
    }

    private (DmodeDungeonItemList Item, DmodeDungeonItemType Type) GenerateDungeonItem(
        int rarity,
        WeaponTypes weaponType,
        List<DmodeDungeonItemList> itemList
    )
    {
        DmodeDungeonItemType type;
        DmodeDungeonItemList item;

        switch (rdm.Next(100))
        {
            case < 80:
                type = DmodeDungeonItemType.Skill;
                item = GenerateDungeonSkill();
                break;
            case < 90:
                type = DmodeDungeonItemType.AbilityCrest;
                item = GenerateDungeonAbilityCrest(rarity);
                break;
            default:
                type = DmodeDungeonItemType.Weapon;
                item = GenerateDungeonWeapon(rarity, weaponType);
                break;
        }

        itemList.AddDmodeItem(item);
        return (item, type);
    }

    private DmodeDungeonItemList GenerateDungeonSkill()
    {
        DmodeDungeonItemData itemData = rdm.Next(
            MasterAsset.DmodeDungeonItemData.Enumerable
                .Where(
                    x =>
                        x.DmodeDungeonItemType == DmodeDungeonItemType.Skill
                        && DmodeHelper.AllowedSkillIds.Contains(x.Id)
                )
                .ToArray()
        );

        DmodeDungeonItemList item =
            new(0, itemData.Id, DmodeDungeonItemState.None, new AtgenOption());

        return item;
    }

    private DmodeDungeonItemList GenerateDungeonWeapon(int rarity, WeaponTypes weaponType)
    {
        DmodeWeapon weapon = rdm.Next(
            MasterAsset.DmodeDungeonItemData.Enumerable
                .Where(
                    x => x.Rarity == rarity && x.DmodeDungeonItemType == DmodeDungeonItemType.Weapon
                )
                .Select(x => MasterAsset.DmodeWeapon[x.Id])
                .Where(x => x.WeaponType == weaponType)
                .ToArray()
        );

        DmodeDungeonItemList item =
            new(0, weapon.Id, DmodeDungeonItemState.None, new AtgenOption());

        item = CalculateAbilities(
            item,
            weapon.StrengthParamGroupId,
            weapon.StrengthSkillGroupId,
            weapon.StrengthAbilityGroupId
        );

        return item;
    }

    private DmodeDungeonItemList GenerateDungeonAbilityCrest(int rarity)
    {
        DmodeDungeonItemData itemData = rdm.Next(
            MasterAsset.DmodeDungeonItemData.Enumerable
                .Where(
                    x =>
                        x.Rarity == rarity
                        && x.DmodeDungeonItemType == DmodeDungeonItemType.AbilityCrest
                )
                .ToArray()
        );

        DmodeDungeonItemList item =
            new(0, itemData.Id, DmodeDungeonItemState.None, new AtgenOption());

        DmodeAbilityCrest crest = MasterAsset.DmodeAbilityCrest[item.item_id];

        item = CalculateAbilities(
            item,
            crest.StrengthParamGroupId,
            0,
            crest.StrengthAbilityGroupId
        );

        return item;
    }

    private DmodeDungeonItemList CalculateAbilities(
        DmodeDungeonItemList item,
        int strengthParamGroupId,
        int strengthSkillGroupId,
        int strengthAbilityGroupId
    )
    {
        if (strengthParamGroupId != 0)
        {
            DmodeStrengthParam param = rdm.Next(
                MasterAsset.DmodeStrengthParam.Enumerable
                    .Where(x => x.StrengthParamGroupId == strengthParamGroupId)
                    .ToArray()
            );

            item.option.strength_param_id = param.Id;
        }

        if (strengthSkillGroupId != 0 && rdm.Next(100) > 50)
        {
            DmodeStrengthSkill skill = rdm.Next(
                MasterAsset.DmodeStrengthSkill.Enumerable
                    .Where(x => x.StrengthSkillGroupId == strengthSkillGroupId && x.SkillId != 0)
                    .ToArray()
            );

            item.option.strength_skill_id = skill.Id;
        }

        if (strengthAbilityGroupId != 0 && rdm.Next(100) > 50)
        {
            DmodeStrengthAbility ability = rdm.Next(
                MasterAsset.DmodeStrengthAbility.Enumerable
                    .Where(
                        x => x.StrengthAbilityGroupId == strengthAbilityGroupId && x.AbilityId != 0
                    )
                    .ToArray()
            );

            item.option.strength_ability_id = ability.Id;
        }

        return item;
    }

    private IEnumerable<AtgenDmodeDropObj> GenerateDrops(
        DmodeAreaInfo areaInfo,
        Func<int, (AtgenDmodeDropList, DmodeDungeonItemType, int)> generateDmodeItemDrop
    )
    {
        int dropCount = areaInfo.DropObjects.Length;

        List<AtgenDmodeDropObj> objs = new();

        for (int i = 0; i < dropCount; i++)
        {
            List<AtgenDmodeDropList> drops = new();

            DropObject type = areaInfo.DropObjects[i];
            switch (type)
            {
                case DropObject.None:
                    throw new InvalidOperationException("None DropObject");
                case DropObject.Barrel:
                    drops.Add(
                        new AtgenDmodeDropList(EntityTypes.DungeonItem, (int)DungeonItem.Hp, 1)
                    );
                    break;
                case DropObject.TreasureChest:
                    int pool = 3; // Max 3 items of rarity 1
                    while (pool > 0)
                    {
                        (AtgenDmodeDropList drop, DmodeDungeonItemType itemType, int rarity) =
                            generateDmodeItemDrop(pool);

                        drops.Add(drop);
                        pool -= rarity switch
                        {
                            1 => 1,
                            < 5 => 2,
                            5 => 3,
                            _ => 1
                        };
                    }

                    break;
                default:
                    throw new UnreachableException();
            }

            objs.Add(new AtgenDmodeDropObj(i + 1, (int)type, drops));
        }

        return objs;
    }

    private IEnumerable<AtgenDmodeEnemy> GenerateEnemies(
        DmodeAreaInfo areaInfo,
        bool isSelectedEntity,
        int baseEnemyLevel,
        int baseBossLevel,
        int plusMin,
        int plusMax,
        Func<int, AtgenDmodeDropList> generateDmodeItemDrop
    )
    {
        int maxEnemyCount = areaInfo.EnemyThemes.Length;

        // TODO: selectively include enemy groups / menaces
        // int enemyCount = isSelectedEntity
        //     ? maxEnemyCount
        //     : rdm.Next((int)Math.Ceiling(maxEnemyCount * 0.8d), maxEnemyCount); // Ceiling so 1 * 0.8 = 1 for boss stages
        int enemyCount = maxEnemyCount;

        List<AtgenDmodeEnemy> dmodeEnemies = new();

        for (int i = 0; i < enemyCount; i++)
        {
            int enemyParam = 0;

            if (isSelectedEntity)
            {
                enemyParam = areaInfo.EnemyParams[i];
            }
            else
            {
                int enemyThemeId = areaInfo.EnemyThemes[i];

                int[] enemyParams = MasterAsset.DmodeEnemyTheme.TryGetValue(
                    enemyThemeId,
                    out DmodeEnemyTheme? enemyTheme
                )
                    ? enemyTheme.AvailableParams
                    : areaInfo.EnemyParams;

                while (enemyParam == 0)
                {
                    enemyParam = rdm.Next(enemyParams);
                }
            }

            EnemyParam paramData = MasterAsset.EnemyParam[enemyParam];
            int enemyLevel = GenerateEnemyLevel(paramData.Id);

            List<AtgenDmodeDropList> enemyDropList = new();

            int numDrops = (int)paramData.Tough * 2;

            // Occasionally grant drops to regular enemies with toughness 0
            if (rdm.Next(100) > 50)
                numDrops += 1;

            for (int j = 0; j < numDrops; j++)
            {
                AtgenDmodeDropList enemyDrop = generateDmodeItemDrop(numDrops);
                enemyDropList.Add(enemyDrop);
            }

            // Give drops to second form
            bool hasSecondForm = paramData.Form2nd != 0;

            dmodeEnemies.Add(
                new AtgenDmodeEnemy(
                    dmodeEnemies.Count,
                    1,
                    enemyLevel,
                    enemyParam,
                    hasSecondForm ? Enumerable.Empty<AtgenDmodeDropList>() : enemyDropList
                )
            );

            GenerateSubEnemies(paramData);

            if (hasSecondForm)
            {
                int level = GenerateEnemyLevel(paramData.Form2nd);
                dmodeEnemies.Add(
                    new AtgenDmodeEnemy(
                        dmodeEnemies.Count,
                        0,
                        level,
                        paramData.Form2nd,
                        enemyDropList
                    )
                );

                EnemyParam secondFormParam = MasterAsset.EnemyParam[paramData.Form2nd];

                GenerateSubEnemies(secondFormParam);
            }
        }

        return dmodeEnemies;

        void GenerateSubEnemies(EnemyParam paramData)
        {
            foreach ((int param, int count) in paramData.Children)
            {
                AddChildEnemies(param, count);
            }

            foreach ((int param, int count) in paramData.Weaks)
            {
                AddChildEnemies(param, count);
            }

            foreach (int param in paramData.Parts)
            {
                if (param != 0)
                    AddChildEnemies(param, 1);
            }
        }

        int GenerateEnemyLevel(int enemyParam)
        {
            EnemyParam paramData = MasterAsset.EnemyParam[enemyParam];

            return paramData.DmodeEnemyLevelType switch
            {
                DmodeEnemyLevelType.None => 1,
                DmodeEnemyLevelType.Enemy
                    => rdm.Next(baseEnemyLevel + plusMin, baseEnemyLevel + plusMax),
                DmodeEnemyLevelType.BossEnemy
                    => rdm.Next(baseBossLevel + plusMin, baseBossLevel + plusMax),
                _ => 1
            };
        }

        void AddChildEnemies(int enemyParam, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int level = GenerateEnemyLevel(enemyParam);
                dmodeEnemies.Add(
                    new AtgenDmodeEnemy(
                        dmodeEnemies.Count,
                        0,
                        level,
                        enemyParam,
                        Enumerable.Empty<AtgenDmodeDropList>()
                    )
                );
            }
        }
    }

    private int GetMinRarity(int floorNum)
    {
        return floorNum switch
        {
            <= 20 => 1,
            <= 30 => 2,
            <= 40 => 3,
            > 40 => 4,
        };
    }

    private int ClampRarityByFloor(int floorNum, int rarity)
    {
        int maxRarity = floorNum switch
        {
            <= 15 => 2,
            <= 25 => 3,
            <= 35 => 4,
            > 35 => 5,
        };

        int minRarity = GetMinRarity(floorNum);

        return Math.Clamp(rarity, minRarity, maxRarity);
    }

    private double GetExpMultiplier(DmodeIngameData ingameData)
    {
        double expMultiplier = 1d;

        DmodeServitorPassiveList? expPassive =
            ingameData.dmode_servitor_passive_list.SingleOrDefault(
                x => x.passive_no == DmodeServitorPassiveType.Exp
            );

        if (expPassive != null)
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.Exp
            ][expPassive.passive_level];

            expMultiplier += level.UpValue / 100;
        }

        return expMultiplier;
    }
}
