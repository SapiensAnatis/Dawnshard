using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Talisman;

[Route("talisman")]
public class TalismanController(
    ITalismanService talismanService,
    IUpdateDataService updateDataService,
    IRewardService rewardService
) : DragaliaControllerBase
{
    [HttpPost("sell")]
    public async Task<DragaliaResult> Sell(TalismanSellRequest request)
    {
        TalismanSellData resp = new();

        resp.delete_data_list = await talismanService.SellTalismans(request.talisman_key_id_list);
        resp.entity_result = rewardService.GetEntityResult();
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("set_lock")]
    public async Task<DragaliaResult> SetLock(TalismanSetLockRequest request)
    {
        TalismanSetLockData resp = new();

        await talismanService.SetLock(request.talisman_key_id, request.is_lock);

        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }
}
