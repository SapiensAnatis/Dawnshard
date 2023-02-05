using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IWeaponService
{
    Task<bool> ValidateCraft(WeaponBodies weaponBodyId);
    Task Craft(WeaponBodies weaponBodyId);

    Task<bool> ValidateBuildup(WeaponBodies id, AtgenBuildupWeaponBodyPieceList buildup);

    Task UnlockBuildup(WeaponBodies id, AtgenBuildupWeaponBodyPieceList buildup);
}
