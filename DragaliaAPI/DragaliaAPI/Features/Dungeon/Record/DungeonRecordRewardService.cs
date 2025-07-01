using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordRewardService(
    IQuestCompletionService questCompletionService,
    IRewardService rewardService,
    IPresentService presentService,
    EventDropService eventDropService,
    IMissionProgressionService missionProgressionService,
    IQuestRepository questRepository,
    ApiContext apiContext,
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
            questData.IsMissionClear3,
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

        foreach (AtgenTreasureRecord record in playRecord.TreasureRecord ?? [])
        {
            if (!session.EnemyList.TryGetValue(record.AreaIdx, out IList<AtgenEnemy>? enemyList))
            {
                logger.LogWarning(
                    "Could not retrieve enemy list for area_idx {idx}",
                    record.AreaIdx
                );
                continue;
            }

            // Sometimes record.enemy is null for boss stages. Give all drops in this case.
            IEnumerable<int> enemyRecord = record.Enemy ?? Enumerable.Repeat(1, enemyList.Count);

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

        double coinMultiplier = AbilityLogic.CalculateCoinMultiplier(
            session.RewardBoostingAbilitiesPerUnit
        );
        double manaMultiplier = AbilityLogic.CalculateManaMultiplier(
            session.RewardBoostingAbilitiesPerUnit
        );

        coinDrop = (int)Math.Round(coinDrop * coinMultiplier);
        manaDrop = (int)Math.Round(manaDrop * manaMultiplier);

        double buildEventMaterialMultiplier = AbilityLogic.CalculateEventMaterialMultiplier(
            session.RewardBoostingAbilitiesPerUnit,
            session.QuestGid
        );

        List<AtgenDropAll> drops = [];

        List<Entity> bonusEntities = [];

        if (buildEventMaterialMultiplier > 1)
        {
            // There is already a drop, so to apply an e.g. 50% bonus, instead of multiplying by 1.5 when adding a new
            // drop, multiply by 0.5 instead.
            double additiveMultiplier = buildEventMaterialMultiplier - 1;

            EventData eventData = MasterAsset.EventData[session.QuestGid];

            foreach (Entity entity in entities)
            {
                if (
                    entity.Type != eventData.ViewEntityType1
                    || entity.Id != eventData.ViewEntityId1
                )
                {
                    // Bonuses only apply to T1 materials
                    continue;
                }

                int boostedQuantity = (int)Math.Round(entity.Quantity * additiveMultiplier);
                Entity bonusEntity = entity with { Quantity = boostedQuantity };
                bonusEntities.Add(bonusEntity);

                // We have no way to communicate 'factor' using the Entity model, so add ourselves to the AtgenDropAll
                // collection instead of merging into 'entities' and waiting to be added below
                AtgenDropAll drop = bonusEntity.ToDropAll();
                drop.Place = 9;
                drops.Add(drop);
            }
        }

        drops.AddRange(entities.Select(x => x.ToDropAll()));

        await rewardService.GrantRewards(entities);
        await rewardService.GrantRewards(bonusEntities);
        await rewardService.GrantReward(new Entity(EntityTypes.Mana, Quantity: manaDrop));
        await rewardService.GrantReward(new Entity(EntityTypes.Rupies, Quantity: coinDrop));

        return (drops, manaDrop, coinDrop);
    }

    public async Task<IList<AtgenDropAll>> ProcessDraconicEssenceDrops(DungeonSession session)
    {
        if (
            !MasterAsset.QuestRewardData.TryGetValue(
                session.QuestId,
                out QuestRewardData? questRewardData
            )
            || questRewardData.DropLimitBreakMaterialId == 0
        )
        {
            return [];
        }

        DbQuest dbRow = await questRepository.GetQuestDataAsync(session.QuestId);

        // We are in this method after the play count has been incremented for the current completion
        // It would be easier to run before, but the play count incrementing function also handles daily resets
        int previousPlayCount = dbRow.DailyPlayCount - session.PlayCount;
        int availableEssences = 3 - previousPlayCount;

        if (availableEssences <= 0)
        {
            return [];
        }

        int rewardQuantity = Math.Min(session.PlayCount, availableEssences);

        Entity essence = new(
            EntityTypes.Material,
            (int)questRewardData.DropLimitBreakMaterialId,
            rewardQuantity
        );

        RewardGrantResult result = await rewardService.GrantReward(essence);

        if (result is not RewardGrantResult.Added)
        {
            presentService.AddPresent(new Present.Present(PresentMessage.QuestDailyBonus, essence));
        }

        return [essence.ToDropAll()];
    }

    public async Task<EventRewardData> ProcessEventRewards(
        PlayRecord playRecord,
        DungeonSession session
    )
    {
        double pointMultiplier = AbilityLogic.CalculateEventPointMultiplier(
            session.RewardBoostingAbilitiesPerUnit,
            eventId: session.QuestGid
        );

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

        return new EventRewardData(
            ScoreMissions: scoreMissions,
            EnemyScoreMissions: enemyScoreMissions,
            TakeAccumulatePoint: totalPoints + boostedPoints + enemyScore,
            TakeBoostAccumulatePoint: boostedPoints,
            PassiveUpList: passiveUpList
        );
    }

    public AtgenFirstMeeting ProcessFirstMeetingRewards(IList<long> connectingViewerIdList)
    {
        // TODO: Replace with social reward check, including limited total quantity and actual checks that
        // it is the first meeting: https://dragalialost.wiki/w/Co-op#Co-op_Social_Rewards
        // This is just a stub implementation to encourage co-op play.
        int quantity = 100 * connectingViewerIdList.Count;

        presentService.AddPresent(
            new Present.Present(
                MessageId: PresentMessage.SocialReward,
                EntityType: EntityTypes.FreeDiamantium,
                EntityId: 0,
                EntityQuantity: quantity
            )
            {
                MessageParamValues = [connectingViewerIdList.Count],
            }
        );

        return new AtgenFirstMeeting()
        {
            Id = 0,
            Type = EntityTypes.FreeDiamantium,
            Headcount = connectingViewerIdList.Count,
            TotalQuantity = quantity,
        };
    }

    public record EventRewardData(
        IEnumerable<AtgenScoreMissionSuccessList> ScoreMissions,
        IEnumerable<AtgenScoringEnemyPointList> EnemyScoreMissions,
        int TakeAccumulatePoint,
        int TakeBoostAccumulatePoint,
        IEnumerable<AtgenEventPassiveUpList> PassiveUpList
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
