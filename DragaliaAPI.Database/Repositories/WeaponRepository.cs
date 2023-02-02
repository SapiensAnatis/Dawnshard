using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public class WeaponRepository : BaseRepository, IWeaponRepository
{
    private readonly ApiContext apiContext;

    public WeaponRepository(ApiContext apiContext)
        : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbWeaponBody> GetWeaponBodies(string accountId)
    {
        return this.apiContext.PlayerWeapons.Where(x => x.DeviceAccountId == accountId);
    }

    public IQueryable<DbWeaponSkin> GetWeaponSkins(string accountId)
    {
        return this.apiContext.PlayerWeaponSkins.Where(x => x.DeviceAccountId == accountId);
    }

    public async Task Add(string accountId, WeaponBodies weaponBodyId)
    {
        await this.apiContext.PlayerWeapons.AddAsync(
            new DbWeaponBody() { DeviceAccountId = accountId, WeaponBodyId = weaponBodyId }
        );
    }

    public async Task AddSkin(string accountId, int weaponSkinId)
    {
        await this.apiContext.PlayerWeaponSkins.AddAsync(
            new DbWeaponSkin() { DeviceAccountId = accountId, WeaponSkinId = weaponSkinId }
        );
    }
}
