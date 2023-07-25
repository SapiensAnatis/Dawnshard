using System.Diagnostics;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon_record")]
public class DungeonRecordController(
    IQuestRepository questRepository,
    IDungeonService dungeonService,
    IInventoryRepository inventoryRepository,
    IUpdateDataService updateDataService,
    ITutorialService tutorialService,
    IMissionProgressionService missionProgressionService,
    ILogger<DungeonRecordController> logger,
    IQuestCompletionService questCompletionService,
    IEventDropService eventDropService,
    IRewardService rewardService,
    IAbilityCrestMultiplierService abilityCrestMultiplierService,
    IUserService userService
) : DragaliaControllerBase
{
    [HttpPost("record")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        return Ok(await BuildResponse(request.dungeon_key, request.play_record, false));
    }

    [HttpPost("record_multi")]
    [Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
    public async Task<DragaliaResult> RecordMulti(DungeonRecordRecordMultiRequest request)
    {
        DungeonRecordRecordData response = await BuildResponse(
            request.dungeon_key,
            request.play_record,
            true
        );

        response.ingame_result_data.play_type = QuestPlayType.Multi;

        return Ok(response);
    }

    private async Task<DungeonRecordRecordData> BuildResponse(
        string dungeonKey,
        PlayRecord playRecord,
        bool isMulti
    )
    {
        // TODO: Turn this method into a service call
        DungeonSession session = await dungeonService.FinishDungeon(dungeonKey);
        logger.LogDebug("session.IsHost: {isHost}", session.IsHost);

        logger.LogDebug("Processing completion of quest {id}", session.QuestData.Id);

        DbQuest? oldQuestData = await questRepository.Quests.SingleOrDefaultAsync(
            x => x.QuestId == session.QuestData.Id
        );

        bool isFirstClear = oldQuestData is null || oldQuestData?.PlayCount == 0;

        if (
            !isFirstClear && (playRecord.is_clear == 1 || session.QuestData.IsPayForceStaminaSingle)
        )
        {
            // TODO: Campaign support

            StaminaType type = StaminaType.None;
            int amount = 0;

            if (isMulti)
            {
                // TODO/NOTE: We do not deduct wings because of the low amount of players playing coop at this point
                // type = StaminaType.Multi;
                // amount = session.QuestData.PayStaminaMulti;
            }
            else
            {
                type = StaminaType.Single;
                amount = session.QuestData.PayStaminaSingle;
            }

            if (type != StaminaType.None && amount != 0)
                await userService.RemoveStamina(type, amount);
        }

        float clear_time = playRecord?.time ?? -1.0f;

        await tutorialService.AddTutorialFlag(1022);

        // oldQuestData and newQuestData actually reference the same object so this is somewhat redundant
        // keeping it for clarity and because oldQuestData is null in some tests
        DbQuest newQuestData = await questRepository.CompleteQuest(
            session.QuestData.Id,
            clear_time
        );

        // Void battle moment :(
        if (session.QuestData.IsPartOfVoidBattleGroups)
            missionProgressionService.OnVoidBattleCleared();

        missionProgressionService.OnQuestCleared(session.QuestData.Id);

        IEnumerable<AtgenFirstClearSet> firstClearRewards = isFirstClear
            ? await questCompletionService.GrantFirstClearRewards(session.QuestData.Id)
            : Enumerable.Empty<AtgenFirstClearSet>();

        bool[] oldMissionStatus =
        {
            newQuestData.IsMissionClear1,
            newQuestData.IsMissionClear2,
            newQuestData.IsMissionClear3
        };

        QuestMissionStatus status = await questCompletionService.CompleteQuestMissions(
            session,
            oldMissionStatus,
            playRecord!
        );

        newQuestData.IsMissionClear1 = status.Missions[0];
        newQuestData.IsMissionClear2 = status.Missions[1];
        newQuestData.IsMissionClear3 = status.Missions[2];

        List<AtgenDropAll> drops = new();
        int manaDrop = 0;
        int coinDrop = 0;

        foreach (
            AtgenTreasureRecord record in playRecord?.treasure_record
                ?? Enumerable.Empty<AtgenTreasureRecord>()
        )
        {
            if (
                !session.EnemyList.TryGetValue(
                    record.area_idx,
                    out IEnumerable<AtgenEnemy>? enemyList
                )
            )
            {
                logger.LogWarning(
                    "Could not retrieve enemy list for area_idx {idx}",
                    record.area_idx
                );
                continue;
            }

            // Sometimes record.enemy is null for boss stages. Give all drops in this case.
            IEnumerable<int> enemyRecord = record.enemy ?? Enumerable.Repeat(1, enemyList.Count());

            foreach (
                EnemyDropList dropList in enemyList
                    .Zip(enemyRecord)
                    .Where(x => x.Second == 1)
                    .SelectMany(x => x.First.enemy_drop_list)
            )
            {
                manaDrop += dropList.mana;
                coinDrop += dropList.coin;
                drops.AddRange(
                    dropList.drop_list.Select(
                        x =>
                            new AtgenDropAll()
                            {
                                type = EntityTypes.Material,
                                id = x.id,
                                quantity = x.quantity,
                            }
                    )
                );
            }
        }

        await inventoryRepository.UpdateQuantity(
            drops.Select(x => new KeyValuePair<Materials, int>((Materials)x.id, x.quantity))
        );

        (IEnumerable<AtgenScoreMissionSuccessList> scoreMissions, int totalPoints) =
            await questCompletionService.CompleteQuestScoreMissions(session, playRecord!);

        IEnumerable<AtgenEventPassiveUpList> eventPassiveDrops =
            await eventDropService.ProcessEventPassiveDrops(session.QuestData);

        double crestMultiplier = await abilityCrestMultiplierService.GetFacilityEventMultiplier(
            session.Party,
            session.QuestData.Gid
        );

        drops.AddRange(
            await eventDropService.ProcessEventMaterialDrops(
                session.QuestData,
                playRecord!,
                crestMultiplier
            )
        );

        await rewardService.GrantReward(new Entity(EntityTypes.Rupies, Quantity: coinDrop));
        await rewardService.GrantReward(new Entity(EntityTypes.Mana, Quantity: manaDrop));

        // Constant for quests with no stamina usage, wip?
        int experience =
            session.QuestData.PayStaminaSingle != 0
                ? session.QuestData.PayStaminaSingle * 10
                : session.QuestData.PayStaminaMulti != 0
                    ? session.QuestData.PayStaminaMulti * 100
                    : 150;

        PlayerLevelResult playerLevelResult = await userService.AddExperience(experience); // TODO: Exp boost

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        return new DungeonRecordRecordData()
        {
            ingame_result_data = new()
            {
                dungeon_key = dungeonKey,
                play_type = QuestPlayType.Default,
                quest_id = session.QuestData.Id,
                reward_record = new()
                {
                    drop_all = drops,
                    first_clear_set = firstClearRewards,
                    take_coin = coinDrop,
                    take_astral_item_quantity = 300,
                    missions_clear_set = status.MissionsClearSet,
                    mission_complete = status.MissionCompleteSet,
                    enemy_piece = new List<AtgenEnemyPiece>(),
                    reborn_bonus = new List<AtgenFirstClearSet>(),
                    quest_bonus_list = new List<AtgenFirstClearSet>(),
                    carry_bonus = new List<AtgenFirstClearSet>(),
                    challenge_quest_bonus_list = new List<AtgenFirstClearSet>(),
                    campaign_extra_reward_list = new List<AtgenFirstClearSet>(),
                    weekly_limit_reward_list = new List<AtgenFirstClearSet>(),
                    take_accumulate_point = totalPoints,
                    player_level_up_fstone = playerLevelResult.RewardedWyrmite
                },
                grow_record = new()
                {
                    take_player_exp = experience,
                    take_chara_exp = 4000,
                    take_mana = manaDrop,
                    bonus_factor = 1,
                    mana_bonus_factor = 1,
                    chara_grow_record = session.Party.Select(
                        x => new AtgenCharaGrowRecord() { chara_id = x.chara_id, take_exp = 240 }
                    ),
                    chara_friendship_list = new List<CharaFriendshipList>()
                },
                start_time = DateTimeOffset.UtcNow,
                end_time = DateTimeOffset.FromUnixTimeSeconds(0),
                current_play_count = 1,
                is_clear = true,
                state = -1,
                is_host = session.IsHost,
                reborn_count = 0,
                helper_list = HelperService.StubData.SupportListData.support_user_list
                    .Skip(1)
                    .Take(1),
                helper_detail_list = new List<AtgenHelperDetailList>()
                {
                    new()
                    {
                        viewer_id = 1001,
                        is_friend = 1,
                        apply_send_status = 0,
                        get_mana_point = 50
                    }
                },
                quest_party_setting_list = session.Party,
                bonus_factor_list = new List<AtgenBonusFactorList>(),
                scoring_enemy_point_list = new List<AtgenScoringEnemyPointList>(),
                score_mission_success_list = scoreMissions,
                event_passive_up_list = eventPassiveDrops,
                clear_time = clear_time,
                is_best_clear_time = clear_time == newQuestData.BestClearTime,
                converted_entity_list = new List<ConvertedEntityList>(),
                dungeon_skip_type = 0,
                total_play_damage = 0,
            },
            update_data_list = updateDataList,
            entity_result = new(),
        };
    }
}
