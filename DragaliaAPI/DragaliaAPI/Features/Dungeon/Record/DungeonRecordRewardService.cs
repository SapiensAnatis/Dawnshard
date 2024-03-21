using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
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
    IMissionProgressionService missionProgressionService,
    IQuestRepository questRepository,
    ILogger<DungeonRecordRewardService> logger
) : IDungeonRecordRewardService
{
    public async Task<(
        QuestMissionStatus MissionStatus,
        IEnumerable<AtgenFirstClearSet> FirstClearRewards
    )> ProcessQuestMissionCompletion(PlayRecord playRecord, DungeonSession session)
    {
        DbQuest questData = await questRepository.GetQuestDataAsync(session.QuestId);

        bool isFirstClear = questData.State < 3;

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
            playRecord
        );

        questData.IsMissionClear1 = status.Missions[0];
        questData.IsMissionClear2 = status.Missions[1];
        questData.IsMissionClear3 = status.Missions[2];

        return (status, firstClearRewards);
    }

    public async Task<(
        IEnumerable<AtgenDropAll> DropList,
        int ManaDrop,
        int CoinDrop
    )> ProcessEnemyDrops(PlayRecord playRecord, DungeonSession session)
    {
        int manaDrop = 0;
        int coinDrop = 0;
        List<Entity> entities = new();

        foreach (
            AtgenTreasureRecord record in playRecord.TreasureRecord
                ?? Enumerable.Empty<AtgenTreasureRecord>()
        )
        {
            if (
                !session.EnemyList.TryGetValue(
                    record.AreaIdx,
                    out IEnumerable<AtgenEnemy>? enemyList
                )
            )
            {
                logger.LogWarning(
                    "Could not retrieve enemy list for area_idx {idx}",
                    record.AreaIdx
                );
                continue;
            }

            // Sometimes record.enemy is null for boss stages. Give all drops in this case.
            IEnumerable<int> enemyRecord = record.Enemy ?? Enumerable.Repeat(1, enemyList.Count());

            foreach (
                EnemyDropList enemyDropList in enemyList
                    .Zip(enemyRecord)
                    .Where(x => x.Second == 1)
                    .SelectMany(x => x.First.EnemyDropList)
            )
            {
                manaDrop += enemyDropList.Mana;
                coinDrop += enemyDropList.Coin;

                entities.AddRange(
                    enemyDropList.DropList.Select(x => new Entity(x.Type, x.Id, x.Quantity))
                );
            }
        }

        entities = entities.Merge().ToList();
        List<AtgenDropAll> drops = entities.Select(x => x.ToDropAll()).ToList();

        await rewardService.GrantRewards(entities);
        await rewardService.GrantReward(new Entity(EntityTypes.Mana, Quantity: manaDrop));
        await rewardService.GrantReward(new Entity(EntityTypes.Rupies, Quantity: coinDrop));

        return (drops, manaDrop, coinDrop);
    }

    public async Task<EventRewardData> ProcessEventRewards(
        PlayRecord playRecord,
        DungeonSession session
    )
    {
        (double materialMultiplier, double pointMultiplier) =
            await abilityCrestMultiplierService.GetEventMultiplier(session.Party, session.QuestGid);

        (
            IEnumerable<AtgenScoreMissionSuccessList> scoreMissions,
            int totalPoints,
            int boostedPoints
        ) = await questCompletionService.CompleteQuestScoreMissions(
            session,
            playRecord,
            pointMultiplier
        );

        if (totalPoints + boostedPoints > 0)
        {
            missionProgressionService.OnEventPointCollected(
                session.QuestGid,
                session.QuestVariation,
                totalPoints + boostedPoints
            );
        }

        (IEnumerable<AtgenScoringEnemyPointList> enemyScoreMissions, int enemyScore) =
            await questCompletionService.CompleteEnemyScoreMissions(session, playRecord);

        if (enemyScore > 0)
        {
            missionProgressionService.OnEventPointCollected(
                session.QuestGid,
                session.QuestVariation,
                enemyScore
            );
        }

        ArgumentNullException.ThrowIfNull(session.QuestData);

        IEnumerable<AtgenEventPassiveUpList> passiveUpList =
            await eventDropService.ProcessEventPassiveDrops(session.QuestData);

        IEnumerable<AtgenDropAll> eventDrops = await eventDropService.ProcessEventMaterialDrops(
            session.QuestData,
            playRecord!,
            materialMultiplier
        );

        return new EventRewardData(
            ScoreMissions: scoreMissions,
            EnemyScoreMissions: enemyScoreMissions,
            TakeAccumulatePoint: totalPoints + boostedPoints + enemyScore,
            TakeBoostAccumulatePoint: boostedPoints,
            PassiveUpList: passiveUpList,
            EventDrops: eventDrops
        );
    }

    public record EventRewardData(
        IEnumerable<AtgenScoreMissionSuccessList> ScoreMissions,
        IEnumerable<AtgenScoringEnemyPointList> EnemyScoreMissions,
        int TakeAccumulatePoint,
        int TakeBoostAccumulatePoint,
        IEnumerable<AtgenEventPassiveUpList> PassiveUpList,
        IEnumerable<AtgenDropAll> EventDrops
    );
}

file static class Extensions
{
    public static IEnumerable<Entity> Merge(this IEnumerable<Entity> source) =>
        source
            .GroupBy(x => new { x.Id, x.Type })
            .Select(group =>
                group.Aggregate(
                    new Entity(group.Key.Type, group.Key.Id, 0),
                    (acc, current) => acc with { Quantity = acc.Quantity + current.Quantity }
                )
            );
}
