using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IWeaponService
{
    Task<bool> ValidateCraft(WeaponBodies weaponBodyId);
    Task Craft(WeaponBodies weaponBodyId);
}
