using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IWeaponRepository
{
    IQueryable<DbWeaponBody> GetWeaponBodies(string accountId);
    Task Add(string accountId, WeaponBodies weaponBodyId);
    Task AddSkin(string accountId, int weaponSkinId);
    IQueryable<DbWeaponSkin> GetWeaponSkins(string accountId);
}
