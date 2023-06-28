using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("event_trade")]
public class EventTradeController : DragaliaControllerBase
{
    private readonly ITradeService tradeService;
    private readonly IUpdateDataService updateDataService;

    public EventTradeController(
        ITradeService tradeService,
        ITradeRepository tradeRepository,
        IUpdateDataService updateDataService
    )
    {
        this.tradeService = tradeService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetListAll()
    {
        EventTradeGetListData resp = new();

        resp.event_trade_list = Enumerable.Empty<EventTradeList>();
        resp.user_event_item_data = new UserEventItemData();
        resp.user_event_trade_list = await this.tradeService.GetUserEventTradeList();

        return Ok(resp);
    }

    /*[HttpPost("trade")]
    public async Task<DragaliaResult> Trade(EventTradeTradeRequest request)
    {
        EventTradeTradeData resp = new();

        await this.tradeService.DoEventTrade(
            request.trade_id,
            request.trade_count
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.event_trade_list = Enumerable.Empty<EventTradeList>();
        resp.user_event_trade_list = await this.tradeService.GetUserEventTradeList();

        return Ok(resp);
    }*/
}
