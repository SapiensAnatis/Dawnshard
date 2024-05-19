using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Features.Quest;

public class QuestService(
    ILogger<QuestService> logger,
    IQuestRepository questRepository,
    TimeProvider timeProvider,
    IQuestCacheService questCacheService,
    IRewardService rewardService,
    IMissionProgressionService missionProgressionService
) : IQuestService
{
    public async Task<(
        bool BestClearTime,
        IEnumerable<AtgenFirstClearSet> Bonus
    )> ProcessQuestCompletion(DungeonSession session, PlayRecord playRecord)
    {
        int questId = session.QuestId;
        int playCount = session.PlayCount;

        DbQuest quest = await questRepository.GetQuestDataAsync(questId);
        quest.State = 3;

        bool isBestClearTime = false;

        if (0 > quest.BestClearTime || quest.BestClearTime > playRecord.Time)
        {
            quest.BestClearTime = playRecord.Time;
            isBestClearTime = true;
        }

        quest.PlayCount += session.PlayCount;

        if (timeProvider.GetLastDailyReset() > quest.LastDailyResetTime)
        {
            logger.LogTrace("Resetting daily play count for quest {questId}", questId);

            quest.DailyPlayCount = 0;
            quest.LastDailyResetTime = timeProvider.GetUtcNow();
        }

        quest.DailyPlayCount += playCount;

        if (timeProvider.GetLastWeeklyReset() > quest.LastWeeklyResetTime)
        {
            logger.LogTrace("Resetting weekly play count for quest {questId}", questId);

            quest.WeeklyPlayCount = 0;
            quest.LastWeeklyResetTime = timeProvider.GetUtcNow();
        }

        quest.WeeklyPlayCount += playCount;

        IEnumerable<AtgenFirstClearSet> questEventRewards = Enumerable.Empty<AtgenFirstClearSet>();

        QuestData questData = MasterAsset.QuestData[questId];

        missionProgressionService.OnQuestCleared(
            questId,
            questData.Gid,
            questData.QuestPlayModeType,
            session.PlayCount,
            quest.PlayCount
        );

        if (questData.IsEventQuest)
        {
            int baseGroupId = MasterAsset.QuestEventGroup[questData.Gid].BaseQuestGroupId;

            await questCacheService.SetQuestGroupQuestIdAsync(baseGroupId, questId);

            questEventRewards = await ProcessQuestEventCompletion(
                baseGroupId,
                questData,
                playCount
            );

            this.ProcessEventQuestMissionProgression(questData, session, playRecord);
        }

        if (questData is { IsEventRegularBattle: true, EventKindType: EventKindType.Build })
            await this.RollExBattleUnlock(quest, questData);

        if (questData.IsEventExBattle)
            quest.IsAppear = false;

        return (isBestClearTime, questEventRewards);
    }

    public async Task<int> GetQuestStamina(int questId, StaminaType type)
    {
        QuestData questData = MasterAsset.QuestData[questId];
        DbQuest questEntity = await questRepository.GetQuestDataAsync(questId);

        if (questData.GroupType == QuestGroupType.MainStory && questEntity.State < 3)
        {
            logger.LogDebug(
                "Attempting first clear of main story quest {questId}: 0 stamina required",
                questId
            );

            return 0;
        }

        return type switch
        {
            StaminaType.Single => questData.PayStaminaSingle,
            // Want to encourage co-op play.
            // Also, `type` here is inferred from endpoint e.g. start_multi, but that doesn't work for time attack.
            StaminaType.Multi
                => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private async Task<IEnumerable<AtgenFirstClearSet>> ProcessQuestEventCompletion(
        int eventGroupId,
        QuestData questData,
        int playCount
    )
    {
        logger.LogTrace("Completing quest for quest group {eventGroupId}", eventGroupId);

        DbQuestEvent questEvent = await questRepository.GetQuestEventAsync(eventGroupId);
        QuestEvent questEventData = MasterAsset.QuestEvent[eventGroupId];

        if (timeProvider.GetLastDailyReset() > questEvent.LastDailyResetTime)
        {
            if (questEventData.QuestBonusType == QuestResetIntervalType.Daily)
            {
                ResetQuestEventBonus(questEvent, questEventData);
            }

            questEvent.DailyPlayCount = 0;
            questEvent.LastDailyResetTime = timeProvider.GetUtcNow();
        }

        questEvent.DailyPlayCount += playCount;

        if (timeProvider.GetLastWeeklyReset() > questEvent.LastWeeklyResetTime)
        {
            if (questEventData.QuestBonusType == QuestResetIntervalType.Weekly)
            {
                ResetQuestEventBonus(questEvent, questEventData);
            }

            questEvent.WeeklyPlayCount = 0;
            questEvent.LastWeeklyResetTime = timeProvider.GetUtcNow();
        }

        questEvent.WeeklyPlayCount += playCount;

        // NOTE: Do we ever need total here?
        missionProgressionService.OnEventGroupCleared(
            eventGroupId,
            questData.VariationType,
            questData.QuestPlayModeType,
            playCount,
            1
        );

        int totalBonusCount = questEvent.QuestBonusReserveCount + questEvent.QuestBonusReceiveCount;
        int remainingBonusCount = questEventData.QuestBonusCount - totalBonusCount;

        if (questEventData.QuestBonusCount == 0 || remainingBonusCount <= 0)
        {
            return Enumerable.Empty<AtgenFirstClearSet>();
        }

        int bonusesToReceive = Math.Min(playCount, remainingBonusCount);

        if (questEventData.QuestBonusReceiveType != QuestBonusReceiveType.AutoReceive)
        {
            questEvent.QuestBonusReserveCount += bonusesToReceive;
            questEvent.QuestBonusReserveTime = timeProvider.GetUtcNow();

            return Enumerable.Empty<AtgenFirstClearSet>();
        }

        questEvent.QuestBonusReceiveCount += bonusesToReceive;

        return (await GenerateBonusDrops(questData.Id, bonusesToReceive)).Select(x =>
            x.ToFirstClearSet()
        );
    }

    private async Task<IEnumerable<Entity>> GenerateBonusDrops(int questId, int count)
    {
        // TODO: Drop gen

        List<Entity> drops = new();

        if (
            !MasterAsset.QuestBonusRewardInfo.TryGetValue(
                questId,
                out QuestBonusReward? questBonusReward
            )
        )
        {
            logger.LogWarning("Failed to retrieve bonus rewards for quest {questId}", questId);
            return drops;
        }

        for (int i = 0; i < count; i++)
        {
            foreach (QuestBonusDrop drop in questBonusReward.Bonuses)
            {
                Entity entity = Entity.FromQuestBonusDrop(drop);
                await rewardService.GrantReward(entity);
                drops.Add(entity);
            }
        }

        return drops;
    }

    public async Task<AtgenReceiveQuestBonus> ReceiveQuestBonus(
        int eventGroupId,
        bool isReceive,
        int count
    )
    {
        DbQuestEvent questEvent = await questRepository.GetQuestEventAsync(eventGroupId);

        int? questId = await questCacheService.GetQuestGroupQuestIdAsync(eventGroupId);

        if (!isReceive || questId == null)
        {
            logger.LogInformation("Cancelling receipt of quest bonus");

            questEvent.QuestBonusReserveCount = 0;
            questEvent.QuestBonusReserveTime = DateTimeOffset.UnixEpoch;

            await questCacheService.RemoveQuestGroupQuestIdAsync(eventGroupId);

            return new AtgenReceiveQuestBonus() { TargetQuestId = questId ?? 0 };
        }

        if (count > questEvent.QuestBonusReserveCount + questEvent.QuestBonusStackCount)
        {
            throw new DragaliaException(
                ResultCode.QuestSelectBonusReceivableLimit,
                "Tried to receive too many drops"
            );
        }

        if (count > questEvent.QuestBonusReserveCount)
        {
            questEvent.QuestBonusStackCount -= count - questEvent.QuestBonusReserveCount;
            questEvent.QuestBonusStackTime =
                questEvent.QuestBonusStackCount == 0
                    ? DateTimeOffset.UnixEpoch
                    : timeProvider.GetUtcNow();

            count = questEvent.QuestBonusReserveCount;
        }

        questEvent.QuestBonusReserveCount = 0;
        questEvent.QuestBonusReserveTime = DateTimeOffset.UnixEpoch;

        questEvent.QuestBonusReceiveCount += count;

        // TODO: bonus factor?
        IEnumerable<AtgenBuildEventRewardEntityList> bonusRewards = (
            await GenerateBonusDrops(questId.Value, count)
        ).Select(x => x.ToBuildEventRewardEntityList());

        // Remove at the end so it doesn't get messed up when erroring
        await questCacheService.RemoveQuestGroupQuestIdAsync(eventGroupId);

        return new AtgenReceiveQuestBonus(questId.Value, count, 1, bonusRewards);
    }

    private void ResetQuestEventBonus(DbQuestEvent questEvent, QuestEvent questEventData)
    {
        questEvent.QuestBonusReceiveCount = 0;

        if (questEventData.QuestBonusReceiveType != QuestBonusReceiveType.AutoReceive)
        {
            if (questEventData.QuestBonusStackCountMax > 0)
            {
                int newStackCount =
                    questEvent.QuestBonusReserveCount + questEvent.QuestBonusStackCount;

                questEvent.QuestBonusStackCount = Math.Min(
                    questEventData.QuestBonusStackCountMax,
                    newStackCount
                );

                questEvent.QuestBonusStackTime = timeProvider.GetUtcNow();
            }

            questEvent.QuestBonusReserveCount = 0;
            questEvent.QuestBonusReserveTime = timeProvider.GetUtcNow();
        }
    }

    private void ProcessEventQuestMissionProgression(
        QuestData questData,
        DungeonSession session,
        PlayRecord playRecord
    )
    {
        if (questData.EventKindType is EventKindType.Build or EventKindType.Clb01)
        {
            foreach (
                AbilityCrests crest in session
                    .Party.SelectMany(x => x.GetAbilityCrestList())
                    .Distinct()
            )
            {
                missionProgressionService.OnEventQuestClearedWithCrest(questData.Gid, crest);
            }
        }

        if (questData.IsEventRegularBattle)
        {
            missionProgressionService.OnEventRegularBattleCleared(
                questData.Gid,
                questData.VariationType
            );
        }
        else if (questData.IsEventChallengeBattle)
        {
            int questScoreMissionId = MasterAsset.QuestRewardData[questData.Id].QuestScoreMissionId;
            int waveCount = MasterAsset.QuestScoreMissionData[questScoreMissionId].WaveCount;

            missionProgressionService.OnEventChallengeBattleCleared(
                questData.Gid,
                questData.VariationType,
                playRecord.Wave >= waveCount,
                questData.Id
            );
        }
        else if (questData.IsEventTrial)
        {
            missionProgressionService.OnEventTrialCleared(questData.Gid, questData.VariationType);
        }
    }

    private async Task RollExBattleUnlock(DbQuest quest, QuestData questData)
    {
        // EX quest IDs are of the format {eventId}0401, e.g. 208260401 for Trick or Treasure which has ID 20826
        int exQuestId = (questData.Gid * 10_000) + 401;

        if (!MasterAsset.QuestData.ContainsKey(exQuestId))
        {
            logger.LogDebug("EX battle quest ID {ExQuestId} was not a valid quest", exQuestId);
            return;
        }

        // Give an EX battle once every 3 clears, or randomly at a 15% chance.
        bool exBattleUnlocked = quest.PlayCount % 3 == 0 || Random.Shared.NextDouble() < 0.15;

        if (!exBattleUnlocked)
            return;

        logger.LogInformation("Unlocking EX battle {ExQuestId}", exQuestId);

        DbQuest exQuest = await questRepository.GetQuestDataAsync(exQuestId);
        exQuest.IsAppear = true;
    }
}
