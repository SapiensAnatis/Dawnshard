using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("event_trade")]
public class EventTradeController : DragaliaControllerBase
{
    private readonly ITradeService tradeService;
    private readonly ITradeRepository tradeRepository;
    private readonly IUpdateDataService updateDataService;

    public EventTradeController(
        ITradeService tradeService,
        ITradeRepository tradeRepository,
        IUpdateDataService updateDataService
    )
    {
        this.tradeService = tradeService;
        this.updateDataService = updateDataService;
        this.tradeRepository = tradeRepository;
    }

    [HttpPost("get_list")]
    public DragaliaResult GetListAll()
    {
        EventTradeGetListData resp = new();

        resp.event_trade_list = Enumerable.Empty<EventTradeList>();
        resp.user_event_item_data = new UserEventItemData();
        resp.user_event_trade_list = Enumerable.Empty<AtgenUserEventTradeList>();

        return Ok(resp);
    }

    /*
    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(EventTradeTradeRequest request)
    {
        // TODO: We don't have events
    }
    */
}
