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
) : IDungeonRecordRewardService
{
    public async Task<(
        QuestMissionStatus MissionStatus,
        IEnumerable<AtgenFirstClearSet> FirstClearRewards
    )> ProcessQuestMissionCompletion(
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

                    await rewardService.GrantReward(reward);
                }
            }
        }

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

        IEnumerable<AtgenEventPassiveUpList> passiveUpList =
            await eventDropService.ProcessEventPassiveDrops(session.QuestData);

        IEnumerable<AtgenDropAll> eventDrops = await eventDropService.ProcessEventMaterialDrops(
            session.QuestData,
            playRecord!,
            materialMultiplier
        );

        return new EventRewardData(
            ScoreMissions: scoreMissions,
            TakeAccumulatePoint: totalPoints + boostedPoints,
            TakeBoostAccumulatePoint: boostedPoints,
            PassiveUpList: passiveUpList,
            EventDrops: eventDrops
        );
    }

    public record EventRewardData(
        IEnumerable<AtgenScoreMissionSuccessList> ScoreMissions,
        int TakeAccumulatePoint,
        int TakeBoostAccumulatePoint,
        IEnumerable<AtgenEventPassiveUpList> PassiveUpList,
        IEnumerable<AtgenDropAll> EventDrops
    );
}
