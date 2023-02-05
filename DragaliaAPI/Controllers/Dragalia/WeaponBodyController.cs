using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("weapon_body")]
public class WeaponBodyController : DragaliaControllerBase
{
    private readonly IWeaponService weaponService;
    private readonly IUpdateDataService updateDataService;
    private readonly ILogger<WeaponBodyController> logger;

    public WeaponBodyController(
        IWeaponService weaponService,
        IUpdateDataService updateDataService,
        ILogger<WeaponBodyController> logger
    )
    {
        this.weaponService = weaponService;
        this.updateDataService = updateDataService;
        this.logger = logger;
    }

    [HttpPost("craft")]
    public async Task<DragaliaResult> Craft(WeaponBodyCraftRequest request)
    {
        if (!await this.weaponService.ValidateCraft(request.weapon_body_id))
        {
            this.logger.LogWarning("Weapon craft request was invalid.");
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        this.logger.LogInformation(
            "Validated request to craft weapon {weapon}",
            request.weapon_body_id
        );

        await this.weaponService.Craft(request.weapon_body_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        WeaponBodyCraftData response = new() { update_data_list = updateDataList };
        return this.Ok(response);
    }

    [HttpPost("buildup_piece")]
    public async Task<DragaliaResult> BuildupPiece(WeaponBodyBuildupPieceRequest request)
    {
        foreach (AtgenBuildupWeaponBodyPieceList buildup in request.buildup_weapon_body_piece_list)
        {
            if (!await this.weaponService.ValidateBuildup(request.weapon_body_id, buildup))
            {
                this.logger.LogWarning("buildup_piece request {request} was invalid", request);
                return this.Code(ResultCode.WeaponBodyBuildupPieceUnablePiece);
            }

            await this.weaponService.UnlockBuildup(request.weapon_body_id, buildup);
        }

        this.logger.LogInformation(
            "Completed request to upgrade weapon {weapon}",
            request.weapon_body_id
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return this.Ok(new WeaponBodyBuildupPieceData() { update_data_list = updateDataList });
    }
}
