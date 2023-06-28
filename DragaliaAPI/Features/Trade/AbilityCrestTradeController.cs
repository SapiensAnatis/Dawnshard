using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;

namespace DragaliaAPI.Features.Trade;

[Route("ability_crest_trade")]
public class AbilityCrestTradeController : DragaliaControllerBase
{
    private readonly ITradeService tradeService;
    private readonly IUpdateDataService updateDataService;
    private readonly IRewardService rewardService;

    public AbilityCrestTradeController(
        ITradeService tradeService,
        IUpdateDataService updateDataService,
        IRewardService rewardService
    )
    {
        this.tradeService = tradeService;
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
    }

    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        AbilityCrestTradeGetListData resp = new();

        resp.ability_crest_trade_list = this.tradeService.GetCurrentAbilityCrestTradeList();
        resp.user_ability_crest_trade_list = await this.tradeService.GetUserAbilityCrestTradeList();
        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.entity_result = this.rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(AbilityCrestTradeTradeRequest request)
    {
        AbilityCrestTradeTradeData resp = new();

        await this.tradeService.DoAbilityCrestTrade(
            request.ability_crest_trade_id,
            request.trade_count
        );

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.entity_result = this.rewardService.GetEntityResult();
        resp.ability_crest_trade_list = this.tradeService.GetCurrentAbilityCrestTradeList();
        resp.user_ability_crest_trade_list = await this.tradeService.GetUserAbilityCrestTradeList();

        return Ok(resp);
    }
}
