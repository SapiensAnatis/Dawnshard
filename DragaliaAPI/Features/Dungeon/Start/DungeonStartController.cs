using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Features.TimeAttack.Validation;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon.Start;

[ApiController]
[Route("dungeon_start")]
public class DungeonStartController(
    IDungeonStartService dungeonStartService,
    IDungeonService dungeonService,
    IOddsInfoService oddsInfoService,
    IMatchingService matchingService,
    ITimeAttackService timeAttackService,
    IUpdateDataService updateDataService,
    ILogger<DungeonStartController> logger
) : DragaliaControllerBase
{
    private const ResultCode FailedValidationCode = ResultCode.CommonInvalidateJson;

    [HttpPost("start")]
    public async Task<DragaliaResult> Start(DungeonStartStartRequest request)
    {
        if (!await dungeonStartService.ValidateStamina(request.quest_id, StaminaType.Single))
            return this.Code(ResultCode.QuestStaminaSingleShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list,
            request.support_viewer_id
        );

        DungeonStartStartData response = await BuildResponse(request.quest_id, ingameData);

        return Ok(response);
    }

    [HttpPost("start_multi")]
    public async Task<DragaliaResult> StartMulti(DungeonStartStartMultiRequest request)
    {
        if (!await dungeonStartService.ValidateStamina(request.quest_id, StaminaType.Multi))
            return this.Code(ResultCode.QuestStaminaMultiShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.quest_id,
            request.party_no_list
        );

        // All time attack quests, regardless of whether they are played solo, appear to use Photon.
        // So we don't need to do this in the solo start endpoints.
        if (
            timeAttackService.GetIsRankedQuest(request.quest_id)
            && !await timeAttackService.SetupRankedClear(request.quest_id, ingameData.party_info)
        )
        {
            return this.Code(FailedValidationCode);
        }

        ingameData.play_type = QuestPlayType.Multi;
        ingameData.is_host = await matchingService.GetIsHost();

        await dungeonService.ModifySession(
            ingameData.dungeon_key,
            session =>
            {
                session.IsHost = ingameData.is_host;
                session.IsMulti = true;
            }
        );

        DungeonStartStartData response = await BuildResponse(request.quest_id, ingameData);

        return Ok(response);
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(DungeonStartStartAssignUnitRequest request)
    {
        if (!await dungeonStartService.ValidateStamina(request.quest_id, StaminaType.Single))
            return this.Code(ResultCode.QuestStaminaSingleShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.quest_id,
            request.request_party_setting_list,
            request.support_viewer_id
        );

        DungeonStartStartData response = await BuildResponse(request.quest_id, ingameData);

        return Ok(response);
    }

    /// <remarks>
    /// Used for repeating time attack solo quests.
    /// </remarks>
    [HttpPost("start_multi_assign_unit")]
    public async Task<DragaliaResult> StartMultiAssignUnit(
        DungeonStartStartMultiAssignUnitRequest request
    )
    {
        if (!await dungeonStartService.ValidateStamina(request.quest_id, StaminaType.Multi))
            return this.Code(ResultCode.QuestStaminaMultiShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.quest_id,
            request.request_party_setting_list
        );

        // All time attack quests, regardless of whether they are played solo, appear to use Photon.
        // So we don't need to do this in the solo start endpoints.
        if (
            timeAttackService.GetIsRankedQuest(request.quest_id)
            && !await timeAttackService.SetupRankedClear(request.quest_id, ingameData.party_info)
        )
        {
            return this.Code(FailedValidationCode);
        }

        ingameData.play_type = QuestPlayType.Multi;
        ingameData.is_host = await matchingService.GetIsHost();

        await dungeonService.ModifySession(
            ingameData.dungeon_key,
            session =>
            {
                session.IsHost = ingameData.is_host;
                session.IsMulti = true;
            }
        );

        DungeonStartStartData response = await BuildResponse(request.quest_id, ingameData);

        return Ok(response);
    }

    private async Task<DungeonStartStartData> BuildResponse(int questId, IngameData ingameData)
    {
        logger.LogDebug("Starting dungeon for quest id {questId}", questId);

        IngameQuestData ingameQuestData = await dungeonStartService.InitiateQuest(questId);

        UpdateDataList updateData = await updateDataService.SaveChangesAsync();

        OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(questId, 0);
        await dungeonService.ModifySession(
            ingameData.dungeon_key,
            session => session.EnemyList[0] = oddsInfo.enemy
        );

        return new()
        {
            ingame_data = ingameData,
            ingame_quest_data = ingameQuestData,
            odds_info = oddsInfo,
            update_data_list = updateData
        };
    }
}
