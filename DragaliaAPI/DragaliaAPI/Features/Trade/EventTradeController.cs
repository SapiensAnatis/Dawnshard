using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
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
        EventTradeGetListData resp = new();

        resp.event_trade_list = tradeService.GetEventTradeList(request.trade_group_id);
        resp.user_event_item_data = new UserEventItemData(); //await eventService.GetUserEventItemData(group.EventId);
        resp.user_event_trade_list = await tradeService.GetUserEventTradeList();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(EventTradeTradeRequest request)
    {
        EventTradeTradeData resp = new();

        await tradeService.DoTrade(TradeType.Event, request.trade_id, request.trade_count);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();
        resp.event_trade_list = tradeService.GetEventTradeList(request.trade_group_id);
        resp.user_event_trade_list = await tradeService.GetUserEventTradeList();
        resp.user_event_item_data = new UserEventItemData(); // await eventService.GetUserEventItemData(group.EventId);

        return Ok(resp);
    }
}
