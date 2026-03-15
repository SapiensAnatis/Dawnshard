using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon")]
public partial class DungeonController(
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

        Log.ProcessingFailRequestForQuest(logger, session.QuestId);

        DungeonFailResponse response = new()
        {
            Result = 1,
            FailQuestDetail = new()
            {
                QuestId = session.QuestId,
                WallId = 0,
                WallLevel = 0,
                IsHost = true,
            },
        };

        Log.SessionIsMultiplayer(logger, session.IsMulti);

        if (session.IsMulti)
        {
            response.FailQuestDetail.IsHost = await matchingService.GetIsHost();
            (response.FailHelperList, response.FailHelperDetailList) =
                await dungeonRecordHelperService.ProcessHelperDataMulti();
        }
        else
        {
            (response.FailHelperList, response.FailHelperDetailList, _) =
                await dungeonRecordHelperService.ProcessHelperDataSolo(session.SupportViewerId);
        }

        Log.FinalResponse(logger, response);

        await dungeonService.RemoveSession(request.DungeonKey, cancellationToken);

        return this.Ok(response);
    }

    [HttpPost("receive_quest_bonus")]
    public async Task<DragaliaResult> ReceiveQuestBonus(
        DungeonReceiveQuestBonusRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonReceiveQuestBonusResponse resp = new()
        {
            ReceiveQuestBonus = await questService.ReceiveQuestBonus(
                request.QuestEventId,
                request.IsReceive,
                request.ReceiveBonusCount
            ),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
            EntityResult = rewardService.GetEntityResult(),
        };

        return Ok(resp);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Processing fail request for quest {QuestId}")]
        public static partial void ProcessingFailRequestForQuest(ILogger logger, int questId);

        [LoggerMessage(LogLevel.Debug, "Session is multiplayer: {IsMulti}")]
        public static partial void SessionIsMultiplayer(ILogger logger, bool isMulti);

        [LoggerMessage(LogLevel.Debug, "Final response: {@Response}")]
        public static partial void FinalResponse(ILogger logger, DungeonFailResponse response);
    }
}
