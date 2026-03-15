using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Weapons;

[Route("weapon_body")]
public partial class WeaponBodyController : DragaliaControllerBase
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
            Log.WeaponCraftRequestWasInvalid(this.logger);
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        Log.ValidatedRequestToCraftWeapon(this.logger, request.WeaponBodyId);

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
        Log.ReceivedRequestToUpgradeWeapon(this.logger, request.WeaponBodyId);

        if (!MasterAsset.WeaponBody.TryGetValue(request.WeaponBodyId, out WeaponBody? bodyData))
        {
            Log.WeaponHadNoMasterAssetEntry(this.logger, request.WeaponBodyId);
            return this.Code(ResultCode.WeaponBodyIsNotPlayable);
        }

        if (!await this.weaponService.CheckOwned(request.WeaponBodyId))
        {
            Log.UserDidNotOwnWeapon(this.logger, request.WeaponBodyId);
            return this.Code(ResultCode.WeaponBodyCraftShortWeaponBody);
        }

        foreach (
            AtgenBuildupWeaponBodyPieceList buildup in request
                .BuildupWeaponBodyPieceList.OrderBy(x => x.BuildupPieceType)
                .ThenBy(x => x.Step)
        )
        {
            ResultCode buildupResult = await this.weaponService.TryBuildup(bodyData, buildup);

            if (buildupResult != ResultCode.Success)
            {
                Log.BuildupPieceRequestWasInvalid(this.logger, request, buildupResult);

                return this.Code(buildupResult);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        Log.CompletedRequestToUpgradeWeapon(this.logger, request.WeaponBodyId);

        return this.Ok(new WeaponBodyBuildupPieceResponse() { UpdateDataList = updateDataList });
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Weapon craft request was invalid.")]
        public static partial void WeaponCraftRequestWasInvalid(ILogger logger);
        [LoggerMessage(LogLevel.Information, "Validated request to craft weapon {weapon}")]
        public static partial void ValidatedRequestToCraftWeapon(ILogger logger, WeaponBodies weapon);
        [LoggerMessage(LogLevel.Debug, "Received request to upgrade weapon {weapon}")]
        public static partial void ReceivedRequestToUpgradeWeapon(ILogger logger, WeaponBodies weapon);
        [LoggerMessage(LogLevel.Error, "Weapon {weapon} had no MasterAsset entry")]
        public static partial void WeaponHadNoMasterAssetEntry(ILogger logger, WeaponBodies weapon);
        [LoggerMessage(LogLevel.Error, "User did not own weapon {weapon}")]
        public static partial void UserDidNotOwnWeapon(ILogger logger, WeaponBodies weapon);
        [LoggerMessage(LogLevel.Error, "buildup_piece request {@request} was invalid: {result}")]
        public static partial void BuildupPieceRequestWasInvalid(ILogger logger, WeaponBodyBuildupPieceRequest request, ResultCode result);
        [LoggerMessage(LogLevel.Information, "Completed request to upgrade weapon {weapon}")]
        public static partial void CompletedRequestToUpgradeWeapon(ILogger logger, WeaponBodies weapon);
    }
}
