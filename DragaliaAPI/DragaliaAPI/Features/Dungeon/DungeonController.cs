using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
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
    IDungeonRecordHelperService dungeonRecordHelperService
) : DragaliaControllerBase
{
    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(DungeonGetAreaOddsRequest request)
    {
        DungeonSession session = await dungeonService.GetDungeon(request.DungeonKey);

        ArgumentNullException.ThrowIfNull(session.QuestData);

        OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(session.QuestData.Id, request.AreaIdx);

        await dungeonService.ModifySession(
            request.DungeonKey,
            s => s.EnemyList[request.AreaIdx] = oddsInfo.Enemy
        );

        return Ok(new DungeonGetAreaOddsResponse() { OddsInfo = oddsInfo });
    }

    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(DungeonFailRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.DungeonKey);

        DungeonFailResponse response =
            new()
            {
                Result = 1,
                FailHelperList = new List<UserSupportList>(),
                FailHelperDetailList = new List<AtgenHelperDetailList>(),
                FailQuestDetail = new()
                {
                    QuestId = session.QuestId,
                    WallId = 0,
                    WallLevel = 0,
                    IsHost = true,
                }
            };

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
