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
    public async Task<DragaliaResult> Sell(
        TalismanSellRequest request,
        CancellationToken cancellationToken
    )
    {
        TalismanSellResponse resp = new();

        resp.DeleteDataList = await talismanService.SellTalismans(request.TalismanKeyIdList);
        resp.EntityResult = rewardService.GetEntityResult();
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("set_lock")]
    public async Task<DragaliaResult> SetLock(
        TalismanSetLockRequest request,
        CancellationToken cancellationToken
    )
    {
        TalismanSetLockResponse resp = new();

        await talismanService.SetLock(request.TalismanKeyId, request.IsLock);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }
}
