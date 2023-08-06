using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("treasure_trade")]
public class TreasureTradeController(
    ITradeService tradeService,
    IUpdateDataService updateDataService,
    IDmodeService dmodeService
) : DragaliaControllerBase
{
    [HttpPost("get_list_all")]
    public async Task<DragaliaResult> GetListAll()
    {
        TreasureTradeGetListAllData resp = new();

        resp.treasure_trade_all_list = tradeService.GetCurrentTreasureTradeList();
        resp.user_treasure_trade_list = await tradeService.GetUserTreasureTradeList();
        resp.dmode_info = await dmodeService.GetInfo();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(TreasureTradeTradeRequest request)
    {
        TreasureTradeTradeData resp = new();

        await tradeService.DoTrade(
            TradeType.Treasure,
            request.treasure_trade_id,
            request.trade_count,
            request.need_unit_list
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.treasure_trade_all_list = tradeService.GetCurrentTreasureTradeList();
        resp.user_treasure_trade_list = await tradeService.GetUserTreasureTradeList();

        return Ok(resp);
    }
}
