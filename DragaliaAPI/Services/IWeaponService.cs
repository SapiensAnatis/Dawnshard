using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IWeaponService
{
    Task<bool> ValidateCraft(string accountId, WeaponBodies weaponBodyId);
    Task Craft(string accountId, WeaponBodies weaponBodyId);
}