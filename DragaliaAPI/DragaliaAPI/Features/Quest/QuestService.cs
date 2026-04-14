using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Features.Quest;

public partial class QuestService(
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
            Log.ResettingDailyPlayCountForQuest(logger, questId);

            quest.DailyPlayCount = 0;
            quest.LastDailyResetTime = timeProvider.GetUtcNow();
        }

        quest.DailyPlayCount += playCount;

        if (timeProvider.GetLastWeeklyReset() > quest.LastWeeklyResetTime)
        {
            Log.ResettingWeeklyPlayCountForQuest(logger, questId);

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
        {
            await this.RollExBattleUnlock(quest, questData);
        }

        if (questData.IsEventExBattle)
        {
            quest.IsAppear = false;
        }

        return (isBestClearTime, questEventRewards);
    }

    public async Task<int> GetQuestStamina(int questId, StaminaType type)
    {
        QuestData questData = MasterAsset.QuestData[questId];
        DbQuest questEntity = await questRepository.GetQuestDataAsync(questId);

        if (questData.GroupType == QuestGroupType.MainStory && questEntity.State < 3)
        {
            Log.AttemptingFirstClearOfMainStoryQuest0StaminaRequired(logger, questId);

            return 0;
        }

        return type switch
        {
            StaminaType.Single => questData.PayStaminaSingle,
            // Want to encourage co-op play.
            // Also, `type` here is inferred from endpoint e.g. start_multi, but that doesn't work for time attack.
            StaminaType.Multi => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    private async Task<IEnumerable<AtgenFirstClearSet>> ProcessQuestEventCompletion(
        int eventGroupId,
        QuestData questData,
        int playCount
    )
    {
        Log.CompletingQuestForQuestGroup(logger, eventGroupId);

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

        int remainingBonusCount;

        if (questEventData.QuestBonusReceiveType == QuestBonusReceiveType.StackSelectReceive)
        {
            // TOTM quests have QuestBonusCount = 1 but QuestBonusReceiveCount can be >1 due to stacking, so we need
            // to use the stack count to view how many are available. This should have just been updated by
            // ResetQuestEventBonus before we got here.

            remainingBonusCount = questEvent.QuestBonusStackCount;
        }
        else
        {
            remainingBonusCount =
                questEventData.QuestBonusCount - questEvent.QuestBonusReceiveCount;
        }

        if (questEventData.QuestBonusCount == 0 || remainingBonusCount <= 0)
        {
            return [];
        }

        int bonusesToReceive = Math.Min(playCount, remainingBonusCount);

        if (questEventData.QuestBonusReceiveType != QuestBonusReceiveType.AutoReceive)
        {
            // For quests which have discretionary bonus claims, sending a quest_event_list entry with a non-zero
            // quest_bonus_reserve_count appears to prompt the client to ask whether a bonus should be claimed.
            // If the player confirms, a request will be sent to /dungeon/receive_quest_bonus.

            questEvent.QuestBonusReserveCount = bonusesToReceive;
            questEvent.QuestBonusReserveTime = timeProvider.GetUtcNow();

            return [];
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
            Log.FailedToRetrieveBonusRewardsForQuest(logger, questId);
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
        QuestEvent questEventData = MasterAsset.QuestEvent[eventGroupId];

        int? questId = await questCacheService.GetQuestGroupQuestIdAsync(eventGroupId);

        if (!isReceive || questId == null)
        {
            // isReceive is false if the player declines to receive the bonus

            Log.CancellingReceiptOfQuestBonus(logger);

            questEvent.QuestBonusReserveCount = 0;
            questEvent.QuestBonusReserveTime = DateTimeOffset.UnixEpoch;

            await questCacheService.RemoveQuestGroupQuestIdAsync(eventGroupId);

            return new() { TargetQuestId = questId ?? 0 };
        }

        if (count > questEvent.QuestBonusReserveCount)
        {
            throw new DragaliaException(
                ResultCode.QuestSelectBonusReceivableLimit,
                "Tried to receive too many drops"
            );
        }

        if (count < 0)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Cannot claim < 0 quest bonuses"
            );
        }

        if (questEventData.QuestBonusReceiveType == QuestBonusReceiveType.StackSelectReceive)
        {
            questEvent.QuestBonusStackCount = Math.Max(0, questEvent.QuestBonusStackCount - count);
            questEvent.QuestBonusStackTime = timeProvider.GetUtcNow();

            // For TOTM the quest bonus display works like
            // Opened chests = QuestBonusReceiveCount
            // Closed chests = (# of resets since QuestBonusStackTime) + QuestBonusStackCount
            // Greyed out closed chests = QuestBonusStackCountMax - (# closed chests) - (# of open chests)
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

        return new(questId.Value, count, 1, bonusRewards);
    }

    private void ResetQuestEventBonus(DbQuestEvent questEvent, QuestEvent questEventData)
    {
        questEvent.QuestBonusReceiveCount = 0;

        if (questEventData.QuestBonusReceiveType != QuestBonusReceiveType.AutoReceive)
        {
            if (questEventData.QuestBonusStackCountMax > 0) // Equivalent to questEventData.QuestBonusReceiveType == StackSelectReceive
            {
                // Note: this has the behaviour of granting 3 bonuses the first time you touch a row (as QuestBonusStackTime
                // will be UnixEpoch).
                // Unsure how this behaved during live service: on the first day of a ToTM quest would you only get one
                // bonus? What if you first played it 2 days after it started running? Not really relevant for Dawnshard,
                // however, as we do not rotate the individual ToTM quests.

                DateTimeOffset stackDailyReset = questEvent.QuestBonusStackTime.GetLastDailyReset();
                DateTimeOffset lastReset = timeProvider.GetLastDailyReset();
                int resetsSinceStackTime = (lastReset - stackDailyReset).Days;

                int newStackCount = questEvent.QuestBonusStackCount + resetsSinceStackTime;

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
                AbilityCrestId crest in session
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
            missionProgressionService.OnEventTrialCleared(
                questData.Gid,
                questData.VariationType,
                questData.Id
            );
        }
    }

    private async Task RollExBattleUnlock(DbQuest quest, QuestData questData)
    {
        // EX quest IDs are of the format {eventId}0401, e.g. 208260401 for Trick or Treasure which has ID 20826
        int exQuestId = (questData.Gid * 10_000) + 401;

        if (!MasterAsset.QuestData.ContainsKey(exQuestId))
        {
            Log.EXBattleQuestIDWasNotAValidQuest(logger, exQuestId);
            return;
        }

        // Give an EX battle once every 3 clears, or randomly at a 15% chance.
        bool exBattleUnlocked = quest.PlayCount % 3 == 0 || Random.Shared.NextDouble() < 0.15;

        if (!exBattleUnlocked)
        {
            return;
        }

        Log.UnlockingEXBattle(logger, exQuestId);

        DbQuest exQuest = await questRepository.GetQuestDataAsync(exQuestId);
        exQuest.IsAppear = true;
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Trace, "Resetting daily play count for quest {questId}")]
        public static partial void ResettingDailyPlayCountForQuest(ILogger logger, int questId);

        [LoggerMessage(LogLevel.Trace, "Resetting weekly play count for quest {questId}")]
        public static partial void ResettingWeeklyPlayCountForQuest(ILogger logger, int questId);

        [LoggerMessage(
            LogLevel.Debug,
            "Attempting first clear of main story quest {questId}: 0 stamina required"
        )]
        public static partial void AttemptingFirstClearOfMainStoryQuest0StaminaRequired(
            ILogger logger,
            int questId
        );

        [LoggerMessage(LogLevel.Trace, "Completing quest for quest group {eventGroupId}")]
        public static partial void CompletingQuestForQuestGroup(ILogger logger, int eventGroupId);

        [LoggerMessage(LogLevel.Warning, "Failed to retrieve bonus rewards for quest {questId}")]
        public static partial void FailedToRetrieveBonusRewardsForQuest(
            ILogger logger,
            int questId
        );

        [LoggerMessage(LogLevel.Information, "Cancelling receipt of quest bonus")]
        public static partial void CancellingReceiptOfQuestBonus(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "EX battle quest ID {ExQuestId} was not a valid quest")]
        public static partial void EXBattleQuestIDWasNotAValidQuest(ILogger logger, int exQuestId);

        [LoggerMessage(LogLevel.Information, "Unlocking EX battle {ExQuestId}")]
        public static partial void UnlockingEXBattle(ILogger logger, int exQuestId);
    }
}
