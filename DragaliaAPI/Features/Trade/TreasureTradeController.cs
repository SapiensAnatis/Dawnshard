using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Trade;

[Route("treasure_trade")]
public class TreasureTradeController : DragaliaControllerBase
{
    private readonly ITreasureTradeService treasureTradeService;
    private readonly ITradeRepository tradeRepository;
    private readonly IUpdateDataService updateDataService;

    public TreasureTradeController(
        ITreasureTradeService treasureTradeService,
        ITradeRepository tradeRepository,
        IUpdateDataService updateDataService
    )
    {
        this.treasureTradeService = treasureTradeService;
        this.updateDataService = updateDataService;
        this.tradeRepository = tradeRepository;
    }

    [HttpPost("get_list_all")]
    public async Task<DragaliaResult> GetListAll()
    {
        TreasureTradeGetListAllData resp = new();

        resp.treasure_trade_all_list = this.treasureTradeService.GetCurrentTradeList();
        resp.user_treasure_trade_list = await this.treasureTradeService.GetUserTradeList();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(TreasureTradeTradeRequest request)
    {
        TreasureTradeTradeData resp = new();

        await this.treasureTradeService.DoTreasureTrade(
            request.treasure_trade_id,
            request.trade_count,
            request.need_unit_list
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.treasure_trade_all_list = this.treasureTradeService.GetCurrentTradeList();
        resp.user_treasure_trade_list = (
            await this.tradeRepository.TreasureTrades.ToListAsync()
        ).Select(x => new UserTreasureTradeList(x.Id, x.Count, x.LastTradeTime));

        return Ok(resp);
    }
}
