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
        TreasureTradeGetListAllResponse resp = new();

        resp.TreasureTradeAllList = tradeService.GetCurrentTreasureTradeList();
        resp.UserTreasureTradeList = await tradeService.GetUserTreasureTradeList();
        resp.DmodeInfo = await dmodeService.GetInfo();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(
        TreasureTradeTradeRequest request,
        CancellationToken cancellationToken
    )
    {
        TreasureTradeTradeResponse resp = new();

        await tradeService.DoTrade(
            TradeType.Treasure,
            request.TreasureTradeId,
            request.TradeCount,
            request.NeedUnitList
        );

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.TreasureTradeAllList = tradeService.GetCurrentTreasureTradeList();
        resp.UserTreasureTradeList = await tradeService.GetUserTreasureTradeList();

        return Ok(resp);
    }
}
