using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class WeaponRepository : IWeaponRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;

    public WeaponRepository(ApiContext apiContext, IPlayerDetailsService playerDetailsService)
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
    }

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbWeaponSkin> WeaponSkins =>
        this.apiContext.PlayerWeaponSkins.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public async Task Add(WeaponBodies weaponBodyId)
    {
        await this.apiContext.PlayerWeapons.AddAsync(
            new DbWeaponBody()
            {
                DeviceAccountId = this.playerDetailsService.AccountId,
                WeaponBodyId = weaponBodyId
            }
        );
    }

    public async Task AddSkin(int weaponSkinId)
    {
        await this.apiContext.PlayerWeaponSkins.AddAsync(
            new DbWeaponSkin()
            {
                DeviceAccountId = this.playerDetailsService.AccountId,
                WeaponSkinId = weaponSkinId
            }
        );
    }

    public async Task<bool> CheckOwnsWeapons(params WeaponBodies[] weaponIds)
    {
        List<WeaponBodies> filtered = weaponIds
            .Where(x => x != Shared.Definitions.Enums.WeaponBodies.Empty)
            .ToList();

        return (
                await this.WeaponBodies
                    .Select(x => x.WeaponBodyId)
                    .Where(x => filtered.Contains(x))
                    .CountAsync()
            ) == filtered.Count;
    }
}
