using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Chara;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordService(
    IDungeonRecordRewardService dungeonRecordRewardService,
    IQuestService questService,
    IUserService userService,
    ITutorialService tutorialService,
    ILogger<DungeonRecordService> logger,
    ICharaService charaService
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
            session.QuestData.Id,
            session.IsHost
        );

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
                total_play_damage = playRecord.total_play_damage,
                clear_time = playRecord.time,
                is_clear = true,
            };

        (
            DbQuest questData,
            ingameResultData.is_best_clear_time,
            ingameResultData.reward_record.quest_bonus_list
        ) = await questService.ProcessQuestCompletion(
            session.QuestData.Id,
            playRecord.time,
            session.PlayCount
        );

        await this.ProcessGrowth(ingameResultData.grow_record, session);
        await this.ProcessStaminaConsumption(session);
        await this.ProcessExperience(
            ingameResultData.grow_record,
            ingameResultData.reward_record,
            session
        );

        (QuestMissionStatus missionStatus, IEnumerable<AtgenFirstClearSet>? firstClearSets) =
            await dungeonRecordRewardService.ProcessQuestMissionCompletion(
                playRecord,
                session,
                questData
            );

        ingameResultData.reward_record.first_clear_set = firstClearSets;
        ingameResultData.reward_record.missions_clear_set = missionStatus.MissionsClearSet;
        ingameResultData.reward_record.mission_complete = missionStatus.MissionCompleteSet;

        (IEnumerable<AtgenDropAll> dropList, int manaDrop, int coinDrop) =
            await dungeonRecordRewardService.ProcessEnemyDrops(playRecord, session);

        ingameResultData.reward_record.take_coin = coinDrop;
        ingameResultData.grow_record.take_mana = manaDrop;
        ingameResultData.reward_record.drop_all.AddRange(dropList);

        (
            IEnumerable<AtgenScoreMissionSuccessList> scoreMissionSuccessList,
            int takeAccumulatePoint,
            int takeBoostAccumulatePoint,
            IEnumerable<AtgenEventPassiveUpList> eventPassiveUpLists,
            IEnumerable<AtgenDropAll> eventDrops
        ) = await dungeonRecordRewardService.ProcessEventRewards(playRecord, session);

        ingameResultData.score_mission_success_list = scoreMissionSuccessList;
        ingameResultData.reward_record.take_accumulate_point = takeAccumulatePoint;
        ingameResultData.reward_record.take_boost_accumulate_point = takeBoostAccumulatePoint;
        ingameResultData.event_passive_up_list = eventPassiveUpLists;
        ingameResultData.reward_record.drop_all.AddRange(eventDrops);

        return ingameResultData;
    }

    private Task ProcessGrowth(GrowRecord growRecord, DungeonSession session)
    {
        // TODO: actual implementation. Extract out into a service at that time
        growRecord.bonus_factor = 1;
        growRecord.mana_bonus_factor = 1;

        return Task.CompletedTask;
    }

    private async Task ProcessStaminaConsumption(DungeonSession session)
    {
        StaminaType type = StaminaType.None;
        int amount = 0;

        if (session.IsMulti)
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
        // Constant for quests with no stamina usage, wip?
        int experience =
            session.QuestData.PayStaminaSingle != 0
                ? session.QuestData.PayStaminaSingle * 10
                : session.QuestData.PayStaminaMulti != 0
                    ? session.QuestData.PayStaminaMulti * 100
                    : 150;

        experience *= session.PlayCount;

        PlayerLevelResult playerLevelResult = await userService.AddExperience(experience); // TODO: Exp boost

        rewardRecord.player_level_up_fstone = playerLevelResult.RewardedWyrmite;
        growRecord.take_player_exp = experience;

        int experiencePerChara = experience * 2;

        List<AtgenCharaGrowRecord> charaGrowRecord = new();

        foreach (PartySettingList chara in session.Party)
        {
            if (chara.chara_id == 0)
                continue;

            await charaService.LevelUpChara(chara.chara_id, experiencePerChara);

            charaGrowRecord.Add(new AtgenCharaGrowRecord(chara.chara_id, experiencePerChara));
            growRecord.take_chara_exp += experiencePerChara;
        }

        growRecord.chara_grow_record = charaGrowRecord;
    }
}
