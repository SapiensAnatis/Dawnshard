using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordService(
    IDungeonRecordMultiService dungeonRecordMultiService,
    IDungeonService dungeonService,
    IQuestRepository questRepository,
    IMissionProgressionService missionProgressionService,
    IRewardService rewardService,
    IMatchingService matchingService,
    IHelperService helperService,
    ILogger<DungeonRecordService> logger
) : IDungeonRecordService
{
    public async Task<IngameResultData> GenerateIngameResultData(
        string dungeonKey,
        PlayRecord playRecord
    )
    {
        DungeonSession session = await dungeonService.FinishDungeon(dungeonKey);

        logger.LogTrace("session.IsHost: {isHost}", session.IsHost);
        logger.LogDebug("Processing completion of quest {id}", session.QuestData.Id);

        IngameResultData ingameResultData =
            new()
            {
                dungeon_key = dungeonKey,
                play_type = QuestPlayType.Default,
                quest_id = session.QuestData.Id,
                is_host = session.IsHost,
                quest_party_setting_list = session.Party,
                start_time = session.StartTime,
                end_time = DateTimeOffset.UtcNow,
                current_play_count = 1,
                reborn_count = playRecord.reborn_count,
                state = -1,
                is_clear = true,
            };

        DbQuest questData = await questRepository.GetQuestDataAsync(session.QuestData.Id);
        questData.State = 3;

        this.ProcessClearTime(ingameResultData, playRecord.time, questData);
        this.ProcessMissionProgression(session, playRecord);
        await this.ProcessQuestRewards(ingameResultData, session, playRecord, questData);
        await this.ProcessGrowth(ingameResultData.grow_record, session);
        await this.ProcessHelperData(ingameResultData, session);

        if (session.IsMulti)
        {
            await this.ProcessHelperDataMulti(ingameResultData);
        }
        else
        {
            await this.ProcessHelperData(ingameResultData, session);
        }

        return ingameResultData;
    }

    private void ProcessClearTime(IngameResultData resultData, float clearTime, DbQuest questEntity)
    {
        bool isBestClearTime = false;

        if (questEntity.BestClearTime > clearTime)
        {
            isBestClearTime = true;
            questEntity.BestClearTime = clearTime;
        }

        resultData.clear_time = clearTime;
        resultData.is_best_clear_time = isBestClearTime;
    }

    private async Task ProcessHelperData(IngameResultData resultData, DungeonSession session)
    {
        if (session.SupportViewerId is null)
            return;

        UserSupportList? supportList = await helperService.GetHelper(session.SupportViewerId.Value);

        if (supportList is not null)
        {
            resultData.helper_list = new List<UserSupportList>() { supportList };

            // TODO: Replace with friend system once implemented
            resultData.helper_detail_list = new List<AtgenHelperDetailList>()
            {
                new()
                {
                    viewer_id = supportList.viewer_id,
                    is_friend = true,
                    apply_send_status = 1,
                    get_mana_point = 50
                }
            };
        }
    }

    // TODO: test with empty weapon / dragon / print slots / etc
    private async Task ProcessHelperDataMulti(IngameResultData resultData)
    {
        IEnumerable<Player> teammates = await matchingService.GetTeammates();

        IEnumerable<UserSupportList> teammateSupportLists =
            await dungeonRecordMultiService.GetTeammateSupportList(teammates);

        // TODO: Replace with friend system once implemented
        IEnumerable<AtgenHelperDetailList> teammateDetailLists = teammates.Select(
            x =>
                new AtgenHelperDetailList()
                {
                    is_friend = true,
                    viewer_id = (ulong)x.ViewerId,
                    get_mana_point = 50,
                    apply_send_status = 0,
                }
        );

        resultData.helper_list = teammateSupportLists;
        resultData.helper_detail_list = teammateDetailLists;
    }

    private Task ProcessGrowth(GrowRecord growRecord, DungeonSession session)
    {
        // TODO: actual implementation. Extract out into a service at that time
        growRecord.take_player_exp = 1;
        growRecord.take_chara_exp = 1;
        growRecord.bonus_factor = 1;
        growRecord.mana_bonus_factor = 1;
        growRecord.chara_grow_record = session.Party.Select(
            x => new AtgenCharaGrowRecord() { chara_id = x.chara_id, take_exp = 1 }
        );

        return Task.CompletedTask;
    }

    private async Task ProcessQuestRewards(
        IngameResultData resultData,
        DungeonSession session,
        PlayRecord playRecord,
        DbQuest questData
    )
    {
        await this.ProcessEnemyDrops(
            resultData.reward_record,
            resultData.grow_record, // Mana drops are added to the grow record. For some reason.
            playRecord,
            session
        );

        await this.ProcessQuestMissionCompletion(resultData.reward_record, playRecord, questData);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "We will need it when we implement mission completion conditions"
    )]
    private async Task ProcessQuestMissionCompletion(
        RewardRecord rewardRecord,
        PlayRecord playRecord,
        DbQuest questData
    )
    {
        List<AtgenMissionsClearSet> missionClearReward = new();
        List<AtgenFirstClearSet> allMissionClearReward = new();

        bool oldMissionClear1 = questData.IsMissionClear1;
        bool oldMissionClear2 = questData.IsMissionClear2;
        bool oldMissionClear3 = questData.IsMissionClear3;

        // TODO: need to implement logic for clearing these based on playRecord
        questData.IsMissionClear1 = true;
        questData.IsMissionClear2 = true;
        questData.IsMissionClear3 = true;
        questData.PlayCount++;
        questData.DailyPlayCount++;
        questData.WeeklyPlayCount++;
        questData.IsAppear = true;

        bool[] newMissionClears = new bool[3]
        {
            questData.IsMissionClear1 && !oldMissionClear1,
            questData.IsMissionClear2 && !oldMissionClear2,
            questData.IsMissionClear3 && !oldMissionClear3,
        };

        bool allMissionsCleared =
            newMissionClears.Any(x => x)
            && questData.IsMissionClear1
            && questData.IsMissionClear2
            && questData.IsMissionClear3;

        // TODO: give actual quest reward instead of 5 wyrmite every time
        foreach (
            (bool missionCleared, int missionNo) in newMissionClears
                .Where(x => x)
                .Select((x, idx) => (x, idx))
        )
        {
            Entity reward = new(EntityTypes.Wyrmite, Quantity: 5);
            await rewardService.GrantReward(reward);
            missionClearReward.Add(reward.ToMissionClearSet(missionNo));
        }

        rewardRecord.missions_clear_set = missionClearReward;

        if (allMissionsCleared)
        {
            Entity reward = new(EntityTypes.Wyrmite, Quantity: 5);
            await rewardService.GrantReward(reward);
            allMissionClearReward.Add(reward.ToFirstClearSet());
        }

        rewardRecord.mission_complete = allMissionClearReward;
    }

    private async Task ProcessEnemyDrops(
        RewardRecord rewardRecord,
        GrowRecord growRecord,
        PlayRecord playRecord,
        DungeonSession session
    )
    {
        int manaDrop = 0;
        int coinDrop = 0;
        List<AtgenDropAll> drops = new();

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
                EnemyDropList enemyDropList in enemyList
                    .Zip(enemyRecord)
                    .Where(x => x.Second == 1)
                    .SelectMany(x => x.First.enemy_drop_list)
            )
            {
                manaDrop += enemyDropList.mana;
                coinDrop += enemyDropList.coin;

                foreach (AtgenDropList dropList in enemyDropList.drop_list)
                {
                    Entity reward = new(dropList.type, dropList.id, dropList.quantity);

                    drops.Add(reward.ToDropAll());

                    await rewardService.GrantReward(reward, log: false);
                }
            }
        }

        rewardRecord.drop_all = drops;
        rewardRecord.take_coin = coinDrop;
        growRecord.take_mana = manaDrop;

        await rewardService.GrantReward(new Entity(EntityTypes.Mana, Quantity: manaDrop));
        await rewardService.GrantReward(new Entity(EntityTypes.Rupies, Quantity: coinDrop));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Will be needed later for advanced mission logic"
    )]
    private void ProcessMissionProgression(DungeonSession session, PlayRecord playRecord)
    {
        if (session.QuestData.IsPartOfVoidBattleGroups)
            missionProgressionService.OnVoidBattleCleared();

        missionProgressionService.OnQuestCleared(session.QuestData.Id);
    }
}
