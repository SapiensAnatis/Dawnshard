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
    public async Task<DragaliaResult> Craft(
        WeaponBodyCraftRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!await this.weaponService.ValidateCraft(request.WeaponBodyId))
        {
            this.logger.LogWarning("Weapon craft request was invalid.");
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        this.logger.LogInformation(
            "Validated request to craft weapon {weapon}",
            request.WeaponBodyId
        );

        await this.weaponService.Craft(request.WeaponBodyId);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        WeaponBodyCraftResponse response = new() { UpdateDataList = updateDataList };
        return this.Ok(response);
    }

    [HttpPost("buildup_piece")]
    public async Task<DragaliaResult> BuildupPiece(
        WeaponBodyBuildupPieceRequest request,
        CancellationToken cancellationToken
    )
    {
        this.logger.LogDebug("Received request to upgrade weapon {weapon}", request.WeaponBodyId);

        if (!MasterAsset.WeaponBody.TryGetValue(request.WeaponBodyId, out WeaponBody? bodyData))
        {
            this.logger.LogError("Weapon {weapon} had no MasterAsset entry", request.WeaponBodyId);
            return this.Code(ResultCode.WeaponBodyIsNotPlayable);
        }

        if (!await this.weaponService.CheckOwned(request.WeaponBodyId))
        {
            this.logger.LogError("User did not own weapon {weapon}", request.WeaponBodyId);
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        foreach (
            AtgenBuildupWeaponBodyPieceList buildup in request
                .BuildupWeaponBodyPieceList.OrderBy(x => x.BuildupPieceType)
                .ThenBy(x => x.Step)
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

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        this.logger.LogInformation(
            "Completed request to upgrade weapon {weapon}",
            request.WeaponBodyId
        );

        return this.Ok(new WeaponBodyBuildupPieceResponse() { UpdateDataList = updateDataList });
    }
}
