using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("treasure_trade")]
public class TreasureTradeController : DragaliaControllerBase
{
    private readonly ITradeService tradeService;
    private readonly IUpdateDataService updateDataService;

    public TreasureTradeController(ITradeService tradeService, IUpdateDataService updateDataService)
    {
        this.tradeService = tradeService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("get_list_all")]
    public async Task<DragaliaResult> GetListAll()
    {
        TreasureTradeGetListAllData resp = new();

        resp.treasure_trade_all_list = this.tradeService.GetCurrentTreasureTradeList();
        resp.user_treasure_trade_list = await this.tradeService.GetUserTreasureTradeList();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(TreasureTradeTradeRequest request)
    {
        TreasureTradeTradeData resp = new();

        await this.tradeService.DoTrade(
            TradeType.Treasure,
            request.treasure_trade_id,
            request.trade_count,
            request.need_unit_list
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.treasure_trade_all_list = this.tradeService.GetCurrentTreasureTradeList();
        resp.user_treasure_trade_list = await this.tradeService.GetUserTreasureTradeList();

        return Ok(resp);
    }
}
