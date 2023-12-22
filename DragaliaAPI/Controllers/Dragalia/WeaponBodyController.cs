using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
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
        this.logger.LogDebug("Received request to upgrade weapon {weapon}", request.weapon_body_id);

        if (!MasterAsset.WeaponBody.TryGetValue(request.weapon_body_id, out WeaponBody? bodyData))
        {
            this.logger.LogError(
                "Weapon {weapon} had no MasterAsset entry",
                request.weapon_body_id
            );
            return this.Code(ResultCode.WeaponBodyIsNotPlayable);
        }

        if (!await this.weaponService.CheckOwned(request.weapon_body_id))
        {
            this.logger.LogError("User did not own weapon {weapon}", request.weapon_body_id);
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        foreach (
            AtgenBuildupWeaponBodyPieceList buildup in request
                .buildup_weapon_body_piece_list.OrderBy(x => x.buildup_piece_type)
                .ThenBy(x => x.step)
        )
        {
            ResultCode buildupResult = await weaponService.TryBuildup(bodyData, buildup);

            if (buildupResult != ResultCode.Success)
            {
                this.logger.LogError(
                    "buildup_piece request {@request} was invalid: {result}",
                    request,
                    buildupResult
                );

                return this.Code(buildupResult);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        this.logger.LogInformation(
            "Completed request to upgrade weapon {weapon}",
            request.weapon_body_id
        );

        return this.Ok(new WeaponBodyBuildupPieceData() { update_data_list = updateDataList });
    }
}
