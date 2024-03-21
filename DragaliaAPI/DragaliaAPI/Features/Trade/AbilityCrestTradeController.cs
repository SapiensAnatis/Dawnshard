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
    public async Task<DragaliaResult> GetList(CancellationToken cancellationToken)
    {
        AbilityCrestTradeGetListResponse resp = new();

        missionProgressionService.OnAbilityCrestTradeViewed();

        resp.AbilityCrestTradeList = tradeService.GetCurrentAbilityCrestTradeList();
        resp.UserAbilityCrestTradeList = await tradeService.GetUserAbilityCrestTradeList();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("trade")]
    public async Task<DragaliaResult> Trade(
        AbilityCrestTradeTradeRequest request,
        CancellationToken cancellationToken
    )
    {
        AbilityCrestTradeTradeResponse resp = new();

        await tradeService.DoAbilityCrestTrade(request.AbilityCrestTradeId, request.TradeCount);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();
        resp.AbilityCrestTradeList = tradeService.GetCurrentAbilityCrestTradeList();
        resp.UserAbilityCrestTradeList = await tradeService.GetUserAbilityCrestTradeList();

        return Ok(resp);
    }
}
