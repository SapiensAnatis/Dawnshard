using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;
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
    public async Task<DragaliaResult> Start(
        DungeonStartStartRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!await dungeonStartService.ValidateStamina(request.QuestId, StaminaType.Single))
            return this.Code(ResultCode.QuestStaminaSingleShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.QuestId,
            request.PartyNoList,
            request.RepeatSetting,
            request.SupportViewerId
        );

        DungeonStartStartResponse response = await BuildResponse(
            request.QuestId,
            ingameData,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost("start_multi")]
    public async Task<DragaliaResult> StartMulti(
        DungeonStartStartMultiRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!await dungeonStartService.ValidateStamina(request.QuestId, StaminaType.Multi))
            return this.Code(ResultCode.QuestStaminaMultiShort);

        IngameData ingameData = await dungeonStartService.GetIngameData(
            request.QuestId,
            request.PartyNoList
        );

        // All time attack quests, regardless of whether they are played solo, appear to use Photon.
        // So we don't need to do this in the solo start endpoints.
        if (
            timeAttackService.GetIsRankedQuest(request.QuestId)
            && !await timeAttackService.SetupRankedClear(request.QuestId, ingameData.PartyInfo)
        )
        {
            return this.Code(FailedValidationCode);
        }

        ingameData.PlayType = QuestPlayType.Multi;
        ingameData.IsHost = await matchingService.GetIsHost();

        await dungeonService.ModifySession(
            ingameData.DungeonKey,
            session =>
            {
                session.IsHost = ingameData.IsHost;
                session.IsMulti = true;
            }
        );

        DungeonStartStartResponse response = await BuildResponse(
            request.QuestId,
            ingameData,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult<DungeonStartStartAssignUnitResponse>> StartAssignUnit(
        DungeonStartStartAssignUnitRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!await dungeonStartService.ValidateStamina(request.QuestId, StaminaType.Single))
            return this.Code(ResultCode.QuestStaminaSingleShort);

        IngameData ingameData = await dungeonStartService.GetAssignUnitIngameData(
            request.QuestId,
            request.RequestPartySettingList,
            request.SupportViewerId,
            request.RepeatSetting
        );

        DungeonStartStartResponse response = await BuildResponse(
            request.QuestId,
            ingameData,
            cancellationToken
        );

        response.IngameData.RepeatState = request.RepeatState;

        return Ok(response);
    }

    /// <remarks>
    /// Used for repeating time attack solo quests.
    /// </remarks>
    [HttpPost("start_multi_assign_unit")]
    public async Task<DragaliaResult> StartMultiAssignUnit(
        DungeonStartStartMultiAssignUnitRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!await dungeonStartService.ValidateStamina(request.QuestId, StaminaType.Multi))
            return this.Code(ResultCode.QuestStaminaMultiShort);

        IngameData ingameData = await dungeonStartService.GetAssignUnitIngameData(
            request.QuestId,
            request.RequestPartySettingList
        );

        // All time attack quests, regardless of whether they are played solo, appear to use Photon.
        // So we don't need to do this in the solo start endpoints.
        if (
            timeAttackService.GetIsRankedQuest(request.QuestId)
            && !await timeAttackService.SetupRankedClear(request.QuestId, ingameData.PartyInfo)
        )
        {
            return this.Code(FailedValidationCode);
        }

        ingameData.PlayType = QuestPlayType.Multi;
        ingameData.IsHost = await matchingService.GetIsHost();

        await dungeonService.ModifySession(
            ingameData.DungeonKey,
            session =>
            {
                session.IsHost = ingameData.IsHost;
                session.IsMulti = true;
            }
        );

        DungeonStartStartResponse response = await BuildResponse(
            request.QuestId,
            ingameData,
            cancellationToken
        );

        return Ok(response);
    }

    private async Task<DungeonStartStartResponse> BuildResponse(
        int questId,
        IngameData ingameData,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Starting dungeon for quest id {questId}", questId);

        IngameQuestData ingameQuestData = await dungeonStartService.InitiateQuest(questId);

        UpdateDataList updateData = await updateDataService.SaveChangesAsync(cancellationToken);

        OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(questId, 0);
        await dungeonService.ModifySession(
            ingameData.DungeonKey,
            session => session.EnemyList[0] = oddsInfo.Enemy
        );

        if (questId == 204270302)
        {
            // Chronos Clash issue workaround: setting the is_rare flag will force him to spawn
            // https://github.com/SapiensAnatis/Dawnshard/issues/515
            oddsInfo.Enemy.First().IsRare = true;
        }

        return new()
        {
            IngameData = ingameData,
            IngameQuestData = ingameQuestData,
            OddsInfo = oddsInfo,
            UpdateDataList = updateData
        };
    }
}
