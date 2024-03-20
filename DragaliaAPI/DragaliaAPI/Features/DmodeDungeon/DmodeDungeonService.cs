using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using DragaliaAPI.Shared.MasterAsset.Models.Enemy;
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

        DbPlayerCharaData chara = await unitRepository.Charas.SingleAsync(x =>
            x.CharaId == charaId
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
            new() { UniqueKey = ingame.UniqueKey, FloorKey = firstFloor.FloorKey };

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

        ingame.RecoveryCount = info.RecoveryCount;
        ingame.RecoveryTime = info.RecoveryTime;

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
        DmodeIngameData ingameData;
        DmodePlayRecord playRecord;
        DmodeFloorData floorData;

        DbPlayerDmodeDungeon dungeon = await dmodeRepository.GetDungeonAsync();

        try
        {
            ingameData = await dmodeCacheService.LoadIngameInfo();
            playRecord = await dmodeCacheService.LoadPlayRecord();
            floorData = await dmodeCacheService.LoadFloorInfo(playRecord.FloorKey);
        }
        catch (DragaliaException ex) when (ex.Code == ResultCode.CommonDbError)
        {
            logger.LogError(ex, "Failed to fetch Kaleidoscape data. Clearing dungeon entry.");
            dungeon.Clear();

            return (dungeon.State, new DmodeIngameResult());
        }

        int floorNum = playRecord.IsFloorIncomplete ? playRecord.FloorNum - 1 : playRecord.FloorNum; // We take the floor num from playRecord due to the additional /floor call on pressing end

        Charas charaId = ingameData.UnitData.CharaId;

        List<AtgenRewardTalismanList> talismans = new();

        if (!isGameOver)
        {
            int talismanAmount = floorNum switch
            {
                < 50 => rdm.Next(1, 3),
                < 60 => rdm.Next(3, 5),
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
                    talisman.TalismanId,
                    talisman.TalismanAbilityId1,
                    talisman.TalismanAbilityId2,
                    talisman.TalismanAbilityId3,
                    talisman.AdditionalHp,
                    talisman.AdditionalAttack
                );

                talismans.Add(talisman);
            }
        }

        Dictionary<DmodeServitorPassiveType, int> passives =
            ingameData.DmodeServitorPassiveList.ToDictionary(x => x.PassiveNo, x => x.PassiveLevel);

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
                FloorNum = floorNum,
                IsRecordFloorNum = floorNum > await dmodeRepository.GetTotalMaxFloorAsync(),
                CharaIdList = new List<Charas> { ingameData.UnitData.CharaId, },
                QuestTime = floorData.DmodeAreaInfo.QuestTime,
                IsViewQuestTime = ingameData.StartFloorNum == 1,
                DmodeScore = floorData.DmodeAreaInfo.DmodeScore,
                RewardTalismanList = talismans,
                TakeDmodePoint1 = (int)
                    Math.Ceiling(floorData.DmodeUnitInfo.TakeDmodePoint1 * pointMultiplier1),
                TakeDmodePoint2 = (int)
                    Math.Ceiling(floorData.DmodeUnitInfo.TakeDmodePoint2 * pointMultiplier2),
                TakePlayerExp = 0,
                PlayerLevelUpFstone = 0,
                ClearState = isGameOver ? 0 : 1
            };

        DbPlayerDmodeChara chara = await dmodeRepository.Charas.SingleAsync(x =>
            x.CharaId == charaId
        );

        if (ingameResult.FloorNum > chara.MaxFloor)
            chara.MaxFloor = ingameResult.FloorNum;

        if (ingameResult.DmodeScore > chara.MaxScore)
            chara.MaxScore = ingameResult.DmodeScore;

        await rewardService.GrantReward(
            new Entity(EntityTypes.DmodePoint, (int)DmodePoint.Point1, ingameResult.TakeDmodePoint1)
        );
        await rewardService.GrantReward(
            new Entity(EntityTypes.DmodePoint, (int)DmodePoint.Point2, ingameResult.TakeDmodePoint2)
        );

        // Clear cache
        await dmodeCacheService.DeletePlayRecord();
        await dmodeCacheService.DeleteFloorInfo(playRecord.FloorKey);
        await dmodeCacheService.DeleteIngameInfo();

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
                floorData = await dmodeCacheService.LoadFloorInfo(playRecord.FloorKey);
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
        dungeon.Floor = floorData.DmodeAreaInfo.FloorNum;
        dungeon.DungeonScore = floorData.DmodeAreaInfo.DmodeScore;
        dungeon.QuestTime = (int)Math.Ceiling(floorData.DmodeAreaInfo.QuestTime);
        dungeon.IsPlayEnd = floorData.IsPlayEnd;

        logger.LogDebug(
            "Generated dmode floor with area info {@dmodeAreaInfo}",
            floorData.DmodeAreaInfo
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
                UniqueKey = ingameData.UniqueKey,
                FloorKey = string.Empty, // Will be set by cache service
                IsEnd = ingameData.StartFloorNum == ingameData.TargetFloorNum,
                IsPlayEnd = false,
                IsViewAreaStartEquipment = ingameData.StartFloorNum != 1, // For the first floor of skip runs
                DmodeAreaInfo = null,
                DmodeUnitInfo = null,
                DmodeDungeonOdds = null
            };

        DmodeQuestFloor floor = MasterAsset.DmodeQuestFloor[ingameData.StartFloorNum];

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

        floorData.DmodeAreaInfo = GenerateAreaInfo(floor, 0, 0, floorData.FloorKey);
        floorData.DmodeUnitInfo = new AtgenDmodeUnitInfo
        {
            Level = 1,
            Exp = 0,
            EquipCrestItemNoSortList = new int[3],
            BagItemNoSortList = new int[10],
            SkillBagItemNoSortList = new int[8],
            DmodeHoldDragonList = Enumerable.Empty<AtgenDmodeHoldDragonList>(),
            TakeDmodePoint1 = 0,
            TakeDmodePoint2 = 0
        };

        floorData.DmodeDungeonOdds = GenerateOddsInfo(
            floor,
            floorData.DmodeAreaInfo,
            editSkillItemList,
            floorData.DmodeUnitInfo,
            ingameData.UnitData.CharaId
        );

        if (floor.FloorNum > 1)
        {
            int newLevel = floor.BaseEnemyLevel + 5;

            floorData.DmodeUnitInfo.Level = newLevel;
            floorData.DmodeUnitInfo.Exp = MasterAsset.DmodeCharaLevel[1000 + newLevel].TotalExp;

            List<AtgenDmodeHoldDragonList> holdDragonList = new();

            int minRarity = GetMinRarity(floor.FloorNum);

            DmodeDungeonItemData[] dragonPool = MasterAsset
                .DmodeDungeonItemData.Enumerable.Where(x =>
                    x.DmodeDungeonItemType == DmodeDungeonItemType.Dragon
                )
                .Where(x =>
                    floorData.DmodeDungeonOdds.DmodeSelectDragonList.All(y =>
                        y.DragonId != (Dragons)x.Id
                    )
                )
                .ToArray();

            for (int i = 0; i < floor.FloorNum; i += 10)
            {
                DmodeDungeonItemData dragon;
                do
                {
                    dragon = rdm.Next(dragonPool);
                } while (holdDragonList.Any(x => (int)x.DragonId == dragon.Id));

                holdDragonList.Add(
                    new AtgenDmodeHoldDragonList() { Count = 0, DragonId = (Dragons)dragon.Id }
                );
            }

            floorData.DmodeUnitInfo.DmodeHoldDragonList = holdDragonList;

            int slot = 0;
            for (int i = 0; i < floor.FloorNum; i += 20)
            {
                DmodeDungeonItemList crest = GenerateDungeonAbilityCrest(minRarity + 1);
                crest.ItemState = DmodeDungeonItemState.EquipCrest;

                floorData.DmodeDungeonOdds.DmodeDungeonItemList.AddDmodeItem(crest);
                floorData.DmodeUnitInfo.EquipCrestItemNoSortList[slot] = crest.ItemNo;
                slot++;
            }

            WeaponTypes weaponType = MasterAsset.CharaData[charaId].WeaponType;
            DmodeDungeonItemList weapon = GenerateDungeonWeapon(minRarity + 1, weaponType);
            weapon.ItemState = DmodeDungeonItemState.EquipWeapon;

            floorData.DmodeDungeonOdds.DmodeDungeonItemList.AddDmodeItem(weapon);
        }

        return floorData;
    }

    private async Task<DmodeFloorData> GenerateSubsequentFloor(
        DmodePlayRecord playRecord,
        DmodeIngameData ingameData
    )
    {
        await dmodeCacheService.StorePlayRecord(playRecord);

        DmodeFloorData previousFloor = await dmodeCacheService.LoadFloorInfo(playRecord.FloorKey);

        AtgenDmodeUnitInfo unitInfo = previousFloor.DmodeUnitInfo;

        int dmodeScore = previousFloor.DmodeAreaInfo.DmodeScore;

        AtgenDmodeEnemy[] enemies =
            previousFloor.DmodeDungeonOdds.DmodeOddsInfo.DmodeEnemy.ToArray();
        int[] enemyKilledStatus = playRecord.DmodeTreasureRecord.Enemy.ToArray();

        double expMultiplier = GetExpMultiplier(ingameData);

        // First, process enemy kills and rewards
        for (int i = 0; i < Math.Min(enemyKilledStatus.Length, enemies.Length); i++)
        {
            if (enemyKilledStatus[i] != 1)
                continue;

            AtgenDmodeEnemy enemy = enemies[i];
            int dmodeEnemyParamGroupId = MasterAsset
                .EnemyParam[enemy.ParamId]
                .DmodeEnemyParamGroupId;

            if (
                !MasterAsset.DmodeEnemyParam.TryGetValue(
                    (dmodeEnemyParamGroupId * 1000) + Math.Min(enemy.Level, 100),
                    out DmodeEnemyParam? enemyParam
                )
            )
            {
                continue;
            }
            unitInfo.Exp += (int)Math.Ceiling(enemyParam.DropExp * expMultiplier);
            unitInfo.TakeDmodePoint1 += enemyParam.DropDmodePoint1;
            unitInfo.TakeDmodePoint2 += enemyParam.DropDmodePoint2;
            dmodeScore += enemyParam.DmodeScore;
        }

        // Now process levels
        DmodeCharaLevel currentLevel = MasterAsset.DmodeCharaLevel[1000 + unitInfo.Level]; // DmodeLevelGroup * 1000 (always 1) + level
        while (
            currentLevel.NecessaryExp != 0
            && unitInfo.Exp > currentLevel.TotalExp + currentLevel.NecessaryExp
        )
        {
            unitInfo.Level++;
            currentLevel = MasterAsset.DmodeCharaLevel[1000 + unitInfo.Level];
        }

        // Now process item list changes
        Dictionary<int, DmodeDungeonItemList> itemList =
            previousFloor.DmodeDungeonOdds.DmodeDungeonItemList.ToDictionary(x => x.ItemNo, x => x);

        foreach (AtgenDmodeDungeonItemStateList stateChange in playRecord.DmodeDungeonItemStateList)
        {
            if (stateChange.State == DmodeDungeonItemState.Sell)
            {
                DmodeDungeonItemData item = MasterAsset.DmodeDungeonItemData[
                    itemList[stateChange.ItemNo].ItemId
                ];
                unitInfo.TakeDmodePoint1 += item.SellDmodePoint1;
                unitInfo.TakeDmodePoint2 += item.SellDmodePoint2;
                itemList[stateChange.ItemNo].ItemState = DmodeDungeonItemState.None;
            }
            else
            {
                itemList[stateChange.ItemNo].ItemState = stateChange.State;
            }
        }

        foreach (
            AtgenDmodeDungeonItemOptionList optionChange in playRecord.DmodeDungeonItemOptionList
        )
        {
            itemList[optionChange.ItemNo].Option.AbnormalStatusInvalidCount =
                optionChange.AbnormalStatusInvalidCount;
        }

        Dictionary<Dragons, int> dragonDict = unitInfo.DmodeHoldDragonList.ToDictionary(
            x => x.DragonId,
            x => x.Count
        );

        foreach (AtgenDmodeDragonUseList dragonUse in playRecord.DmodeDragonUseList)
        {
            dragonDict[dragonUse.DragonId] += dragonUse.UseCount;
        }

        // Now process dragon selection (if applicable)
        if (playRecord.SelectDragonNo != 0)
        {
            List<AtgenDmodeSelectDragonList> dragonSelection =
                previousFloor.DmodeDungeonOdds.DmodeSelectDragonList.ToList();
            AtgenDmodeSelectDragonList dragon = dragonSelection[playRecord.SelectDragonNo - 1];
            if (dragon.IsRare)
            {
                if (dragon.PayDmodePoint1 != 0 && dragon.PayDmodePoint1 > unitInfo.TakeDmodePoint1)
                {
                    throw new DragaliaException(
                        ResultCode.DmodeDungeonFloorDragonParamInconsistent,
                        "Not enough points"
                    );
                }

                unitInfo.TakeDmodePoint1 -= dragon.PayDmodePoint1;

                if (dragon.PayDmodePoint2 != 0 && dragon.PayDmodePoint2 > unitInfo.TakeDmodePoint2)
                {
                    throw new DragaliaException(
                        ResultCode.DmodeDungeonFloorDragonParamInconsistent,
                        "Not enough points"
                    );
                }

                unitInfo.TakeDmodePoint2 -= dragon.PayDmodePoint2;
            }

            dragonDict[dragon.DragonId] = 0;
        }

        // Transfer properties from play record
        unitInfo.BagItemNoSortList = playRecord.BagItemNoSortList;
        unitInfo.EquipCrestItemNoSortList = playRecord.EquipCrestItemNoSortList;
        unitInfo.SkillBagItemNoSortList = playRecord.SkillBagItemNoSortList;
        unitInfo.DmodeHoldDragonList = dragonDict.Select(x => new AtgenDmodeHoldDragonList(
            x.Key,
            x.Value
        ));

        // Generate random floor data
        DmodeQuestFloor floor = MasterAsset.DmodeQuestFloor[Math.Min(playRecord.FloorNum + 1, 60)];

        AtgenDmodeAreaInfo areaInfo = GenerateAreaInfo(
            floor,
            playRecord.QuestTime,
            dmodeScore,
            previousFloor.FloorKey
        );
        AtgenDmodeDungeonOdds odds = GenerateOddsInfo(
            floor,
            areaInfo,
            itemList.Values.ToList(),
            unitInfo,
            ingameData.UnitData.CharaId
        );

        DmodeFloorData floorData =
            new()
            {
                UniqueKey = previousFloor.UniqueKey,
                FloorKey = previousFloor.FloorKey, // Done so we can always reference the current floor
                IsEnd = playRecord.FloorNum == ingameData.TargetFloorNum, // Game ignores this
                IsPlayEnd = playRecord.IsFloorIncomplete, // Game ignores this (it only checks the one in DmodeIngameData)
                IsViewAreaStartEquipment = false, // This can never be true as it only applies to the first floor after skipping
                DmodeAreaInfo = areaInfo,
                DmodeUnitInfo = unitInfo,
                DmodeDungeonOdds = odds
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

        List<DmodeDungeonArea> areas = MasterAsset
            .DmodeDungeonArea.Enumerable.Where(x => x.ThemeGroupId == floorTheme)
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
        DmodeDungeonTheme theme = MasterAsset.DmodeDungeonTheme[dmodeAreaInfo.CurrentAreaThemeId];

        DmodeDungeonArea area = MasterAsset.DmodeDungeonArea[dmodeAreaInfo.CurrentAreaId];
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
                int rarityPool = rdm.Next(101);
                rarityPool += toughness * 10;

                int rarity = rarityPool switch
                {
                    > 98 => 5,
                    > 93 => 4,
                    > 85 => 3,
                    > 75 => 2,
                    _ => 1
                };

                rarity = ClampRarityByFloor(dmodeAreaInfo.FloorNum, rarity);

                (DmodeDungeonItemList item, DmodeDungeonItemType type) = GenerateDungeonItem(
                    rarity,
                    weaponType,
                    itemList
                );

                return new AtgenDmodeDropList(EntityTypes.DmodeDungeonItem, item.ItemNo, 1);
            }
        );

        IEnumerable<AtgenDmodeDropObj> dmodeDropObjs = GenerateDrops(
            areaInfo,
            remainingPool =>
            {
                int rarityPool = rdm.Next(101);
                rarityPool += dmodeAreaInfo.FloorNum;

                int rarity = rarityPool switch
                {
                    > 98 when remainingPool >= 3 => 5,
                    > 93 when remainingPool >= 2 => 4,
                    > 85 when remainingPool >= 2 => 3,
                    > 75 when remainingPool >= 2 => 2,
                    _ => 1
                };

                rarity = ClampRarityByFloor(dmodeAreaInfo.FloorNum, rarity);

                (DmodeDungeonItemList item, DmodeDungeonItemType type) = GenerateDungeonItem(
                    rarity,
                    weaponType,
                    itemList
                );

                return (
                    new AtgenDmodeDropList(EntityTypes.DmodeDungeonItem, item.ItemNo, 1),
                    type,
                    rarity
                );
            }
        );

        DmodeOddsInfo oddsInfo = new(dmodeDropObjs, dmodeEnemies);

        List<AtgenDmodeSelectDragonList> dragonList = new();

        if (theme.BossAppear && dmodeUnitInfo.DmodeHoldDragonList.Count() < 8)
        {
            List<int> alreadyRolledDragonIds = new();
            IEnumerable<int> alreadyOwnedDragonIds = dmodeUnitInfo.DmodeHoldDragonList.Select(x =>
                (int)x.DragonId
            );

            DmodeDungeonItemData[] dragonPool = MasterAsset
                .DmodeDungeonItemData.Enumerable.Where(x =>
                    x.DmodeDungeonItemType == DmodeDungeonItemType.Dragon
                )
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

                selectDragon.SelectDragonNo = i + 1;
                selectDragon.DragonId = (Dragons)dragon.DungeonItemTargetId;

                int dragonRarity = MasterAsset.DragonData[selectDragon.DragonId].Rarity;

                if (rdm.Next(101) > 80 && dragonList.Any(x => !x.IsRare))
                {
                    selectDragon.IsRare = true;
                    selectDragon.PayDmodePoint1 = rdm.Next(
                        (int)Math.Ceiling(floor.Id * 0.5d),
                        (int)Math.Ceiling(floor.Id * 2d) + 1
                    );
                    selectDragon.PayDmodePoint2 = 0;
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

        switch (rdm.Next(101))
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
            MasterAsset
                .DmodeDungeonItemData.Enumerable.Where(x =>
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
            MasterAsset
                .DmodeDungeonItemData.Enumerable.Where(x =>
                    x.Rarity == rarity && x.DmodeDungeonItemType == DmodeDungeonItemType.Weapon
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
            MasterAsset
                .DmodeDungeonItemData.Enumerable.Where(x =>
                    x.Rarity == rarity
                    && x.DmodeDungeonItemType == DmodeDungeonItemType.AbilityCrest
                )
                .ToArray()
        );

        DmodeDungeonItemList item =
            new(0, itemData.Id, DmodeDungeonItemState.None, new AtgenOption());

        DmodeAbilityCrest crest = MasterAsset.DmodeAbilityCrest[item.ItemId];

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
                MasterAsset
                    .DmodeStrengthParam.Enumerable.Where(x =>
                        x.StrengthParamGroupId == strengthParamGroupId
                    )
                    .ToArray()
            );

            item.Option.StrengthParamId = param.Id;
        }

        if (strengthSkillGroupId != 0 && rdm.Next(101) > 50)
        {
            DmodeStrengthSkill skill = rdm.Next(
                MasterAsset
                    .DmodeStrengthSkill.Enumerable.Where(x =>
                        x.StrengthSkillGroupId == strengthSkillGroupId && x.SkillId != 0
                    )
                    .ToArray()
            );

            item.Option.StrengthSkillId = skill.Id;
        }

        if (strengthAbilityGroupId != 0 && rdm.Next(101) > 50)
        {
            DmodeStrengthAbility ability = rdm.Next(
                MasterAsset
                    .DmodeStrengthAbility.Enumerable.Where(x =>
                        x.StrengthAbilityGroupId == strengthAbilityGroupId && x.AbilityId != 0
                    )
                    .ToArray()
            );

            item.Option.StrengthAbilityId = ability.Id;
        }

        return item;
    }

    private static List<AtgenDmodeDropObj> GenerateDrops(
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

    private List<AtgenDmodeEnemy> GenerateEnemies(
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
            if (rdm.Next(101) > 50)
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
                    true,
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
                        false,
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
                    => rdm.Next(baseEnemyLevel + plusMin, baseEnemyLevel + plusMax + 1),
                DmodeEnemyLevelType.BossEnemy
                    => rdm.Next(baseBossLevel + plusMin, baseBossLevel + plusMax + 1),
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
                        false,
                        level,
                        enemyParam,
                        Enumerable.Empty<AtgenDmodeDropList>()
                    )
                );
            }
        }
    }

    private static int GetMinRarity(int floorNum)
    {
        return floorNum switch
        {
            <= 20 => 1,
            <= 30 => 2,
            <= 40 => 3,
            > 40 => 4,
        };
    }

    private static int ClampRarityByFloor(int floorNum, int rarity)
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

    private static double GetExpMultiplier(DmodeIngameData ingameData)
    {
        double expMultiplier = 1d;

        DmodeServitorPassiveList? expPassive = ingameData.DmodeServitorPassiveList.SingleOrDefault(
            x => x.PassiveNo == DmodeServitorPassiveType.Exp
        );

        if (expPassive != null)
        {
            DmodeServitorPassiveLevel level = DmodeHelper.PassiveLevels[
                DmodeServitorPassiveType.Exp
            ][expPassive.PassiveLevel];

            expMultiplier += level.UpValue / 100;
        }

        return expMultiplier;
    }
}
