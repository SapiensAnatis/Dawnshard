using DragaliaAPI.Features.Album;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using static DragaliaAPI.Features.Tutorial.TutorialService;

namespace DragaliaAPI.Features.Dungeon.Record;

internal partial class DungeonRecordService(
    IDungeonRecordRewardService dungeonRecordRewardService,
    IQuestService questService,
    IUserService userService,
    ITutorialService tutorialService,
    ICharaService charaService,
    IRewardService rewardService,
    IAlbumService albumService,
    ILogger<DungeonRecordService> logger
) : IDungeonRecordService
{
    public async Task<IngameResultData> GenerateIngameResultData(
        string dungeonKey,
        PlayRecord playRecord,
        DungeonSession session
    )
    {
        await tutorialService.AddTutorialFlag(1022);

        Log.ProcessingCompletionOfQuest(logger, session.QuestId, session.IsHost);

        IngameResultData ingameResultData = new()
        {
            DungeonKey = dungeonKey,
            PlayType = QuestPlayType.Default,
            QuestId = session.QuestId,
            IsHost = session.IsHost,
            QuestPartySettingList = session.Party,
            StartTime = session.StartTime,
            EndTime = DateTimeOffset.UtcNow,
            CurrentPlayCount = 1,
            RebornCount = playRecord.RebornCount,
            TotalPlayDamage = playRecord.TotalPlayDamage,
            ClearTime = playRecord.Time,
            IsClear = true,
        };

        await this.ProcessStaminaConsumption(session);

        (QuestMissionStatus missionStatus, IEnumerable<AtgenFirstClearSet> firstClearSets) =
            await dungeonRecordRewardService.ProcessQuestMissionCompletion(playRecord, session);

        (ingameResultData.IsBestClearTime, ingameResultData.RewardRecord.QuestBonusList) =
            await questService.ProcessQuestCompletion(session, playRecord);

        await this.ProcessExperience(
            ingameResultData.GrowRecord,
            ingameResultData.RewardRecord,
            session
        );

        await this.ProcessCharaHonors(playRecord, session);

        ingameResultData.RewardRecord.FirstClearSet = firstClearSets;
        ingameResultData.RewardRecord.MissionsClearSet = missionStatus.MissionsClearSet;
        ingameResultData.RewardRecord.MissionComplete = missionStatus.MissionCompleteSet;

        IList<AtgenDropAll> essenceDrops =
            await dungeonRecordRewardService.ProcessDraconicEssenceDrops(session);

        (IEnumerable<AtgenDropAll> dropList, int manaDrop, int coinDrop) =
            await dungeonRecordRewardService.ProcessEnemyDrops(playRecord, session);

        ingameResultData.RewardRecord.TakeCoin = coinDrop;
        ingameResultData.GrowRecord.TakeMana = manaDrop;
        ingameResultData.RewardRecord.DropAll.AddRange(dropList);
        ingameResultData.RewardRecord.DropAll.AddRange(essenceDrops);

        (
            IEnumerable<AtgenScoreMissionSuccessList> scoreMissionSuccessList,
            IEnumerable<AtgenScoringEnemyPointList> enemyScoreMissionList,
            int takeAccumulatePoint,
            int takeBoostAccumulatePoint,
            IEnumerable<AtgenEventPassiveUpList> eventPassiveUpLists
        ) = await dungeonRecordRewardService.ProcessEventRewards(playRecord, session);

        ingameResultData.ScoreMissionSuccessList = scoreMissionSuccessList;
        ingameResultData.RewardRecord.TakeAccumulatePoint = takeAccumulatePoint;
        ingameResultData.ScoringEnemyPointList = enemyScoreMissionList;
        ingameResultData.RewardRecord.TakeBoostAccumulatePoint = takeBoostAccumulatePoint;
        ingameResultData.EventPassiveUpList = eventPassiveUpLists;

        ingameResultData.ConvertedEntityList = rewardService
            .GetConvertedEntityList()
            .Select(x => x.ToConvertedEntityList())
            .Merge()
            .ToList();

        if (
            session.QuestId == TutorialQuestIds.AvenueToPowerBeginner
            && await tutorialService.GetCurrentTutorialStatus() == TutorialStatusIds.CoopTutorial
        )
        {
            logger.LogDebug("Detected co-op tutorial: updating tutorial status");
            await tutorialService.UpdateTutorialStatus(20501);
        }

        return ingameResultData;
    }

    private async Task ProcessCharaHonors(PlayRecord playRecord, DungeonSession session)
    {
        /*
         * From the wiki (https://dragalialost.wiki/w/Medals), the criteria for receiving medals is:
         *
         * 1. By clearing certain quests without using skip tickets, your adventurers can earn medals.
         *
         * 2. If an adventurer's HP falls to zero, they will not gain any medals. If you use any revives or continues,
         * none of your adventurers will gain any medals.
         *
         * 3. When playing co-op, only adventurers you yourself control will be eligible for medals. Also, if a player
         * disconnects immediately after a quest begins and your own adventurer(s) are forced to take their place,
         * those adventurers will not be able to obtain any medals.
         *
         * 4. Adventurers that are only temporarily part of your roster can only earn medals once they have permanently
         * joined you.
         *
         * 1) and 2) are handled here, 3) is hopefully handled by the client and what it chooses to send as
         * session.Party, 4) is handled in AlbumService (though irrelevant until raids are implemented).
         */

        if (playRecord.RebornCount > 0)
        {
            Log.IneligibleRevivesUsed(logger);
            return;
        }

        // This check is redundant, since the only quests that allow continuing but don't allow reviving are event
        // quests that don't grant medals anyway. If a quest has revives enabled these will always be used before the
        // player is given an option to use a continue.

        if (playRecord.PlayContinueCount > 0)
        {
            Log.IneligibleContinuesUsed(logger);
            return;
        }

        if (session.IsSkipTicket)
        {
            Log.IneligibleSkipTicketsUsed(logger);
            return;
        }

        List<Charas> honorRecipients = new(8); // SinDom

        foreach (PartySettingList chara in session.Party)
        {
            if (chara.CharaId == 0)
            {
                continue;
            }

            if (!playRecord.LiveUnitNoList.Contains(chara.UnitNo))
            {
                Log.CharacterNotInLiveUnitNoList(logger, chara.CharaId, chara.UnitNo);
                continue;
            }

            honorRecipients.Add(chara.CharaId);
        }

        if (honorRecipients.Count > 0)
        {
            await albumService.GrantCharaHonors(honorRecipients, session.QuestId);
        }
    }

    private async Task ProcessStaminaConsumption(DungeonSession session)
    {
        StaminaType type = session.IsMulti ? StaminaType.Multi : StaminaType.Single;
        int amount = await questService.GetQuestStamina(session.QuestId, type);

        amount *= session.PlayCount;

        if (amount != 0)
        {
            await userService.RemoveStamina(type, amount);
        }
    }

    private async Task ProcessExperience(
        GrowRecord growRecord,
        RewardRecord rewardRecord,
        DungeonSession session
    )
    {
        ArgumentNullException.ThrowIfNull(session.QuestData);

        // Constant for quests with no stamina usage, wip?
        int experience =
            session.QuestData.PayStaminaSingle != 0 ? session.QuestData.PayStaminaSingle * 10
            : session.QuestData.PayStaminaMulti != 0 ? session.QuestData.PayStaminaMulti * 100
            : 150;

        experience *= session.PlayCount;

        PlayerLevelResult playerLevelResult = await userService.AddExperience(experience); // TODO: Exp boost

        rewardRecord.PlayerLevelUpFstone = playerLevelResult.RewardedWyrmite;
        growRecord.TakePlayerExp = playerLevelResult.ExpGained;

        int experiencePerChara = experience * 2;

        List<AtgenCharaGrowRecord> charaGrowRecord = new();

        foreach (PartySettingList chara in session.Party)
        {
            if (chara.CharaId == 0)
            {
                continue;
            }

            await charaService.LevelUpChara(chara.CharaId, experiencePerChara);

            charaGrowRecord.Add(new AtgenCharaGrowRecord(chara.CharaId, experiencePerChara));
            growRecord.TakeCharaExp += experiencePerChara;
        }

        growRecord.CharaGrowRecord = charaGrowRecord;
        growRecord.BonusFactor = 1;
        growRecord.ManaBonusFactor = 1;
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Processing completion of quest {Id}. isHost: {IsHost}")]
        public static partial void ProcessingCompletionOfQuest(
            ILogger<DungeonRecordService> logger,
            int id,
            bool isHost
        );

        [LoggerMessage(LogLevel.Information, "Quest clear is ineligible for medals: revives used")]
        public static partial void IneligibleRevivesUsed(ILogger<DungeonRecordService> logger);

        [LoggerMessage(
            LogLevel.Information,
            "Quest clear is ineligible for medals: continues used"
        )]
        public static partial void IneligibleContinuesUsed(ILogger<DungeonRecordService> logger);

        [LoggerMessage(
            LogLevel.Information,
            "Quest clear is ineligible for medals: skip tickets used"
        )]
        public static partial void IneligibleSkipTicketsUsed(ILogger<DungeonRecordService> logger);

        [LoggerMessage(
            LogLevel.Information,
            "Character {CharaId} (no. {UnitNo}) is ineligible for medals: not in live_unit_no_list"
        )]
        public static partial void CharacterNotInLiveUnitNoList(
            ILogger<DungeonRecordService> logger,
            Charas charaId,
            int unitNo
        );
    }
}
