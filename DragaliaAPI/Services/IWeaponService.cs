using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Services;

public interface IWeaponService
{
    Task<bool> ValidateCraft(WeaponBodies weaponBodyId);
    Task Craft(WeaponBodies weaponBodyId);
    Task<bool> CheckOwned(WeaponBodies weaponBodyId);

    /// <summary>
    /// Try to buildup a weapon.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data</param>
    /// <param name="buildup"></param>
    /// <returns></returns>
    Task<ResultCode> TryBuildup(WeaponBody body, AtgenBuildupWeaponBodyPieceList buildup);
}
