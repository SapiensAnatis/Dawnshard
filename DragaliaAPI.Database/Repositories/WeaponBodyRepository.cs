using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public class WeaponBodyRepository : IWeaponBodyRepository
{
    private readonly ApiContext apiContext;

    public WeaponBodyRepository(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbWeaponBody> GetWeaponBodies(string accountId)
    {
        return this.apiContext.PlayerWeapons.Where(x => x.DeviceAccountId == accountId);
    }
}
