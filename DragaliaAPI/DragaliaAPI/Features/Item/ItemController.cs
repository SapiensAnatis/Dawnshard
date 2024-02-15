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
        ItemGetListData resp = new();

        resp.item_list = await itemService.GetItemList();

        return Ok(resp);
    }

    [HttpPost("use_recovery_stamina")]
    public async Task<DragaliaResult> UseRecoveryStamina(ItemUseRecoveryStaminaRequest request)
    {
        ItemUseRecoveryStaminaData resp = new();

        resp.recover_data = await itemService.UseItems(request.use_item_list);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = new EntityResult();

        return Ok(resp);
    }
}
