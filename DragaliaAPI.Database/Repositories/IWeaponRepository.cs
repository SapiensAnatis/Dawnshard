using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IWeaponRepository
{
    IQueryable<DbWeaponBody> WeaponBodies { get; }
    IQueryable<DbWeaponSkin> WeaponSkins { get; }
    Task Add(WeaponBodies weaponBodyId);
    Task AddSkin(int weaponSkinId);
    Task<bool> CheckOwnsWeapons(params WeaponBodies[] weaponIds);
    Task<DbWeaponBody?> FindAsync(WeaponBodies id);
    public Task AddPassiveAbility(WeaponBodies id, int abilityNo);
}
