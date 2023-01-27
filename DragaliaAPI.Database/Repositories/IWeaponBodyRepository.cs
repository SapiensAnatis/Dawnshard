using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IWeaponBodyRepository
{
    IQueryable<DbWeaponBody> GetWeaponBodies(string accountId);
}
