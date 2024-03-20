using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Item;

[Route("item")]
public class ItemController(IUpdateDataService updateDataService, IItemService itemService)
    : DragaliaControllerBase
{
    [HttpPost("get_list")]
    public async Task<DragaliaResult> GetList()
    {
        ItemGetListResponse resp = new();

        resp.ItemList = await itemService.GetItemList();

        return Ok(resp);
    }

    [HttpPost("use_recovery_stamina")]
    public async Task<DragaliaResult> UseRecoveryStamina(
        ItemUseRecoveryStaminaRequest request,
        CancellationToken cancellationToken
    )
    {
        ItemUseRecoveryStaminaResponse resp = new();

        resp.RecoverData = await itemService.UseItems(request.UseItemList);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = new EntityResult();

        return Ok(resp);
    }
}
