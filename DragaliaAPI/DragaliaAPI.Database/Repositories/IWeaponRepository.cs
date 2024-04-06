using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Database.Repositories;

public interface IWeaponRepository
{
    IQueryable<DbWeaponBody> WeaponBodies { get; }
    IQueryable<DbWeaponSkin> WeaponSkins { get; }
    IQueryable<DbWeaponPassiveAbility> GetPassiveAbilities(Charas id);
    Task Add(WeaponBodies weaponBodyId);
    Task AddSkin(int weaponSkinId);
    Task<bool> CheckOwnsWeapons(params WeaponBodies[] weaponIds);
    Task<DbWeaponBody?> FindAsync(WeaponBodies id);
    Task AddPassiveAbility(WeaponBodies id, WeaponPassiveAbility passiveAbility);
}
