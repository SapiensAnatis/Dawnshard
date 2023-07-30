using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordRewardService(
    IQuestCompletionService questCompletionService,
    IRewardService rewardService,
    IAbilityCrestMultiplierService abilityCrestMultiplierService,
    IEventDropService eventDropService,
    ILogger<DungeonRecordRewardService> logger
)
{
    public async Task<IngameResultData> ProcessQuestRewards(
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

        await this.ProcessQuestMissionCompletion(
            resultData.reward_record,
            playRecord,
            session,
            questData
        );

        await this.ProcessEventRewards(resultData, playRecord, session);

        return resultData;
    }

    private async Task ProcessQuestMissionCompletion(
        RewardRecord rewardRecord,
        PlayRecord playRecord,
        DungeonSession session,
        DbQuest questData
    )
    {
        bool isFirstClear = questData.PlayCount == 0;

        IEnumerable<AtgenFirstClearSet> firstClearRewards = isFirstClear
            ? await questCompletionService.GrantFirstClearRewards(questData.QuestId)
            : Enumerable.Empty<AtgenFirstClearSet>();

        bool[] oldMissionStatus =
        {
            questData.IsMissionClear1,
            questData.IsMissionClear2,
            questData.IsMissionClear3
        };

        QuestMissionStatus status = await questCompletionService.CompleteQuestMissions(
            session,
            oldMissionStatus,
            playRecord!
        );

        questData.IsMissionClear1 = status.Missions[0];
        questData.IsMissionClear2 = status.Missions[1];
        questData.IsMissionClear3 = status.Missions[2];

        rewardRecord.first_clear_set = firstClearRewards;
        rewardRecord.mission_complete = status.MissionCompleteSet;
        rewardRecord.missions_clear_set = status.MissionsClearSet;
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

    private async Task ProcessEventRewards(
        IngameResultData ingameResultData,
        PlayRecord playRecord,
        DungeonSession session
    )
    {
        (double materialMultiplier, double pointMultiplier) =
            await abilityCrestMultiplierService.GetEventMultiplier(
                session.Party,
                session.QuestData.Gid
            );

        (
            IEnumerable<AtgenScoreMissionSuccessList> scoreMissions,
            int totalPoints,
            int boostedPoints
        ) = await questCompletionService.CompleteQuestScoreMissions(
            session,
            playRecord!,
            pointMultiplier
        );

        ingameResultData.score_mission_success_list = scoreMissions;
        ingameResultData.reward_record.take_accumulate_point = totalPoints + boostedPoints;
        ingameResultData.reward_record.take_boost_accumulate_point = boostedPoints;

        ingameResultData.event_passive_up_list = await eventDropService.ProcessEventPassiveDrops(
            session.QuestData
        );

        ingameResultData.reward_record.drop_all.AddRange(
            await eventDropService.ProcessEventMaterialDrops(
                session.QuestData,
                playRecord!,
                materialMultiplier
            )
        );
    }
}
