using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("event_trade")]
public class EventTradeController(
    ITradeService tradeService,
    IUpdateDataService updateDataService,
    IRewardService rewardService
) : DragaliaControllerBase
{
    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetListAll(EventTradeGetListRequest request)
    {
        EventTradeGetListResponse resp = new();

        resp.EventTradeList = tradeService.GetEventTradeList(request.TradeGroupId);
        resp.UserEventItemData = new UserEventItemData(); //await eventService.GetUserEventItemData(group.EventId);
        resp.UserEventTradeList = await tradeService.GetUserEventTradeList();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(
        EventTradeTradeRequest request,
        CancellationToken cancellationToken
    )
    {
        EventTradeTradeResponse resp = new();

        await tradeService.DoTrade(TradeType.Event, request.TradeId, request.TradeCount);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();
        resp.EventTradeList = tradeService.GetEventTradeList(request.TradeGroupId);
        resp.UserEventTradeList = await tradeService.GetUserEventTradeList();
        resp.UserEventItemData = new UserEventItemData(); // await eventService.GetUserEventItemData(group.EventId);

        return Ok(resp);
    }
}
