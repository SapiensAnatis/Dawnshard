using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Services.Photon;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon")]
public class DungeonController(
    IDungeonService dungeonService,
    IOddsInfoService oddsInfoService,
    IQuestService questService,
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IMatchingService matchingService,
    IDungeonRecordHelperService dungeonRecordHelperService,
    ILogger<DungeonController> logger
) : DragaliaControllerBase
{
    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(
        DungeonGetAreaOddsRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonSession session = await dungeonService.GetSession(
            request.DungeonKey,
            cancellationToken
        );

        ArgumentNullException.ThrowIfNull(session.QuestData);

        OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(session.QuestData.Id, request.AreaIdx);

        await dungeonService.ModifySession(
            request.DungeonKey,
            s => s.EnemyList[request.AreaIdx] = oddsInfo.Enemy,
            cancellationToken
        );

        await dungeonService.SaveSession(cancellationToken);

        return Ok(new DungeonGetAreaOddsResponse() { OddsInfo = oddsInfo });
    }

    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(
        DungeonFailRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonSession session = await dungeonService.GetSession(
            request.DungeonKey,
            cancellationToken
        );

        logger.LogDebug("Processing fail request for quest {QuestId}", session.QuestId);

        DungeonFailResponse response =
            new()
            {
                Result = 1,
                FailQuestDetail = new()
                {
                    QuestId = session.QuestId,
                    WallId = 0,
                    WallLevel = 0,
                    IsHost = true,
                }
            };

        logger.LogDebug("Session is multiplayer: {IsMulti}", session.IsMulti);

        if (session.IsMulti)
        {
            response.FailQuestDetail.IsHost = await matchingService.GetIsHost();
            (response.FailHelperList, response.FailHelperDetailList) =
                await dungeonRecordHelperService.ProcessHelperDataMulti();
        }
        else
        {
            (response.FailHelperList, response.FailHelperDetailList) =
                await dungeonRecordHelperService.ProcessHelperDataSolo(session.SupportViewerId);
        }

        logger.LogDebug("Final response: {@Response}", response);

        await dungeonService.RemoveSession(request.DungeonKey, cancellationToken);

        return this.Ok(response);
    }

    [HttpPost("receive_quest_bonus")]
    public async Task<DragaliaResult> ReceiveQuestBonus(
        DungeonReceiveQuestBonusRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonReceiveQuestBonusResponse resp = new();

        resp.ReceiveQuestBonus = await questService.ReceiveQuestBonus(
            request.QuestEventId,
            request.IsReceive,
            request.ReceiveBonusCount
        );

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }
}
