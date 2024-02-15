using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Trade;

[Route("ability_crest_trade")]
public class AbilityCrestTradeController(
    ITradeService tradeService,
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IMissionProgressionService missionProgressionService
) : DragaliaControllerBase
{
    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        AbilityCrestTradeGetListData resp = new();

        missionProgressionService.OnAbilityCrestTradeViewed();

        resp.ability_crest_trade_list = tradeService.GetCurrentAbilityCrestTradeList();
        resp.user_ability_crest_trade_list = await tradeService.GetUserAbilityCrestTradeList();
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(AbilityCrestTradeTradeRequest request)
    {
        AbilityCrestTradeTradeData resp = new();

        await tradeService.DoAbilityCrestTrade(request.ability_crest_trade_id, request.trade_count);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();
        resp.ability_crest_trade_list = tradeService.GetCurrentAbilityCrestTradeList();
        resp.user_ability_crest_trade_list = await tradeService.GetUserAbilityCrestTradeList();

        return Ok(resp);
    }
}
