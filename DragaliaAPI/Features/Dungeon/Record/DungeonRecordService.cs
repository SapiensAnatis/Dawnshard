using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using PhotonPlayer = DragaliaAPI.Photon.Shared.Models.Player;

namespace DragaliaAPI.Features.Dungeon.Record;

public class DungeonRecordService(
    IDungeonRecordHelperService dungeonRecordHelperService,
    IDungeonRecordRewardService dungeonRecordRewardService,
    IQuestRepository questRepository,
    IMissionProgressionService missionProgressionService,
    IUserService userService,
    ILogger<DungeonRecordService> logger
) : IDungeonRecordService
{
    public async Task<IngameResultData> GenerateIngameResultData(
        string dungeonKey,
        PlayRecord playRecord,
        DungeonSession session
    )
    {
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
                state = -1,
                is_clear = true,
                total_play_damage = playRecord.total_play_damage,
            };

        DbQuest questData = await questRepository.GetQuestDataAsync(session.QuestData.Id);
        questData.State = 3;

        this.ProcessClearTime(ingameResultData, playRecord.time, questData);
        this.ProcessMissionProgression(session);
        await this.ProcessGrowth(ingameResultData.grow_record, session);
        await this.ProcessStaminaConsumption(session);
        await this.ProcessPlayerLevel(ingameResultData.reward_record, session);

        ingameResultData = await dungeonRecordRewardService.ProcessQuestRewards(
            ingameResultData,
            session,
            playRecord,
            questData
        );

        ingameResultData = session.IsMulti
            ? await dungeonRecordHelperService.ProcessHelperDataSolo(
                ingameResultData,
                session.SupportViewerId
            )
            : await dungeonRecordHelperService.ProcessHelperDataMulti(ingameResultData);

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

    private void ProcessMissionProgression(DungeonSession session)
    {
        if (session.QuestData.IsPartOfVoidBattleGroups)
            missionProgressionService.OnVoidBattleCleared();

        missionProgressionService.OnQuestCleared(session.QuestData.Id);
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

        if (type != StaminaType.None && amount != 0)
            await userService.RemoveStamina(type, amount);
    }

    private async Task ProcessPlayerLevel(RewardRecord rewardRecord, DungeonSession session)
    {
        // Constant for quests with no stamina usage, wip?
        int experience =
            session.QuestData.PayStaminaSingle != 0
                ? session.QuestData.PayStaminaSingle * 10
                : session.QuestData.PayStaminaMulti != 0
                    ? session.QuestData.PayStaminaMulti * 100
                    : 150;

        PlayerLevelResult playerLevelResult = await userService.AddExperience(experience); // TODO: Exp boost

        rewardRecord.player_level_up_fstone = playerLevelResult.RewardedWyrmite;
    }
}
