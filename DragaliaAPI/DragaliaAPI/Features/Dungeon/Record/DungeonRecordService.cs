using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using static DragaliaAPI.Services.Game.TutorialService;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordService(
    IDungeonRecordRewardService dungeonRecordRewardService,
    IQuestService questService,
    IUserService userService,
    ITutorialService tutorialService,
    ICharaService charaService,
    IRewardService rewardService,
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

        logger.LogDebug(
            "Processing completion of quest {id}. isHost: {isHost}",
            session.QuestId,
            session.IsHost
        );

        IngameResultData ingameResultData =
            new()
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

        (QuestMissionStatus missionStatus, IEnumerable<AtgenFirstClearSet>? firstClearSets) =
            await dungeonRecordRewardService.ProcessQuestMissionCompletion(playRecord, session);

        (ingameResultData.IsBestClearTime, ingameResultData.RewardRecord.QuestBonusList) =
            await questService.ProcessQuestCompletion(session, playRecord);

        await this.ProcessExperience(
            ingameResultData.GrowRecord,
            ingameResultData.RewardRecord,
            session
        );

        ingameResultData.RewardRecord.FirstClearSet = firstClearSets;
        ingameResultData.RewardRecord.MissionsClearSet = missionStatus.MissionsClearSet;
        ingameResultData.RewardRecord.MissionComplete = missionStatus.MissionCompleteSet;

        (IEnumerable<AtgenDropAll> dropList, int manaDrop, int coinDrop) =
            await dungeonRecordRewardService.ProcessEnemyDrops(playRecord, session);

        ingameResultData.RewardRecord.TakeCoin = coinDrop;
        ingameResultData.GrowRecord.TakeMana = manaDrop;
        ingameResultData.RewardRecord.DropAll.AddRange(dropList);

        (
            IEnumerable<AtgenScoreMissionSuccessList> scoreMissionSuccessList,
            IEnumerable<AtgenScoringEnemyPointList> enemyScoreMissionList,
            int takeAccumulatePoint,
            int takeBoostAccumulatePoint,
            IEnumerable<AtgenEventPassiveUpList> eventPassiveUpLists,
            IEnumerable<AtgenDropAll> eventDrops
        ) = await dungeonRecordRewardService.ProcessEventRewards(playRecord, session);

        ingameResultData.ScoreMissionSuccessList = scoreMissionSuccessList;
        ingameResultData.RewardRecord.TakeAccumulatePoint = takeAccumulatePoint;
        ingameResultData.ScoringEnemyPointList = enemyScoreMissionList;
        ingameResultData.RewardRecord.TakeBoostAccumulatePoint = takeBoostAccumulatePoint;
        ingameResultData.EventPassiveUpList = eventPassiveUpLists;
        ingameResultData.RewardRecord.DropAll.AddRange(eventDrops);

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

    private async Task ProcessStaminaConsumption(DungeonSession session)
    {
        StaminaType type = session.IsMulti ? StaminaType.Multi : StaminaType.Single;
        int amount = await questService.GetQuestStamina(session.QuestId, type);

        amount *= session.PlayCount;

        if (type != StaminaType.None && amount != 0)
            await userService.RemoveStamina(type, amount);
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
            session.QuestData.PayStaminaSingle != 0
                ? session.QuestData.PayStaminaSingle * 10
                : session.QuestData.PayStaminaMulti != 0
                    ? session.QuestData.PayStaminaMulti * 100
                    : 150;

        experience *= session.PlayCount;

        PlayerLevelResult playerLevelResult = await userService.AddExperience(experience); // TODO: Exp boost

        rewardRecord.PlayerLevelUpFstone = playerLevelResult.RewardedWyrmite;
        growRecord.TakePlayerExp = experience;

        int experiencePerChara = experience * 2;

        List<AtgenCharaGrowRecord> charaGrowRecord = new();

        foreach (PartySettingList chara in session.Party)
        {
            if (chara.CharaId == 0)
                continue;

            await charaService.LevelUpChara(chara.CharaId, experiencePerChara);

            charaGrowRecord.Add(new AtgenCharaGrowRecord(chara.CharaId, experiencePerChara));
            growRecord.TakeCharaExp += experiencePerChara;
        }

        growRecord.CharaGrowRecord = charaGrowRecord;
        growRecord.BonusFactor = 1;
        growRecord.ManaBonusFactor = 1;
    }
}
