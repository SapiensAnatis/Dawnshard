using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("weapon_body")]
public class WeaponBodyController : DragaliaControllerBase
{
    private readonly IWeaponService weaponService;
    private readonly IUpdateDataService updateDataService;

    public WeaponBodyController(
        IWeaponService weaponService,
        IUpdateDataService updateDataService
    )
    {
        this.weaponService = weaponService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("craft")]
    public async Task<DragaliaResult> Craft(WeaponBodyCraftRequest request)
    {
        if (!await this.weaponService.ValidateCraft(DeviceAccountId, request.weapon_body_id))
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);

        await this.weaponService.Craft(DeviceAccountId, request.weapon_body_id);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(DeviceAccountId);

        await this.updateDataService.SaveChangesAsync();

        WeaponBodyCraftData response = new() { update_data_list = updateDataList };
        return this.Ok(response);
    }
}
