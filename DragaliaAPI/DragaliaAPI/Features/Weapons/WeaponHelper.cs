using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Weapons;

internal static class WeaponHelper
{
    private static readonly FrozenDictionary<WeaponTypes, WeaponBodies> DefaultWeapons =
        new Dictionary<WeaponTypes, WeaponBodies>()
        {
            { WeaponTypes.Sword, WeaponBodies.BattlewornSword },
            { WeaponTypes.Katana, WeaponBodies.BattlewornBlade },
            { WeaponTypes.Dagger, WeaponBodies.BattlewornDagger },
            { WeaponTypes.Axe, WeaponBodies.BattlewornAxe },
            { WeaponTypes.Lance, WeaponBodies.BattlewornLance },
            { WeaponTypes.Bow, WeaponBodies.BattlewornBow },
            { WeaponTypes.Rod, WeaponBodies.BattlewornWand },
            { WeaponTypes.Cane, WeaponBodies.BattlewornStaff },
            { WeaponTypes.Gun, WeaponBodies.BattlewornManacaster },
        }.ToFrozenDictionary();

    public static WeaponBodies GetDefaultWeaponId(WeaponTypes type) => DefaultWeapons[type];
}
