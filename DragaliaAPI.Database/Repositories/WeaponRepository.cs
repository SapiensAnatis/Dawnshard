using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class WeaponRepository : IWeaponRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<WeaponRepository> logger;

    public WeaponRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<WeaponRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
        this.logger = logger;
    }

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbWeaponSkin> WeaponSkins =>
        this.apiContext.PlayerWeaponSkins.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbWeaponPassiveAbility> WeaponPassiveAbilities =>
        this.apiContext.PlayerPassiveAbilities.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public IQueryable<DbWeaponPassiveAbility> GetPassiveAbilities(WeaponBodies id)
    {
        WeaponBody data = MasterAsset.WeaponBody.Get(id);

        IEnumerable<int> searchIds = MasterAsset.WeaponPassiveAbility.Enumerable
            .Where(x => x.WeaponType == data.WeaponType && x.ElementalType == data.ElementalType)
            .Select(x => x.Id);

        return this.apiContext.PlayerPassiveAbilities.Where(
            x =>
                x.DeviceAccountId == this.playerDetailsService.AccountId
                && searchIds.Contains(x.WeaponPassiveAbilityId)
        );
    }

    public async Task Add(WeaponBodies weaponBodyId)
    {
        this.logger.LogDebug("Adding weapon {weapon}", weaponBodyId);

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
        this.logger.LogDebug("Adding weapon skin {skin}", weaponSkinId);

        if (
            await this.apiContext.PlayerWeaponSkins.FindAsync(
                this.playerDetailsService.AccountId,
                weaponSkinId
            )
            is not null
        )
        {
            this.logger.LogDebug("Weapon skin was already owned.");
            return;
        }

        await this.apiContext.PlayerWeaponSkins.AddAsync(
            new DbWeaponSkin()
            {
                DeviceAccountId = this.playerDetailsService.AccountId,
                WeaponSkinId = weaponSkinId,
                GetTime = DateTimeOffset.UtcNow
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

    public async Task<DbWeaponBody?> FindAsync(WeaponBodies id) =>
        await this.apiContext.PlayerWeapons.FindAsync(this.playerDetailsService.AccountId, id);

    public async Task AddPassiveAbility(WeaponBodies id, WeaponPassiveAbility passiveAbility)
    {
        this.logger.LogDebug(
            "Unlocking passive ability no {no}",
            passiveAbility.WeaponPassiveAbilityNo
        );

        DbWeaponBody? entity = await this.FindAsync(id);
        ArgumentNullException.ThrowIfNull(entity);

        List<int> passiveList = entity.UnlockWeaponPassiveAbilityNoList.ToList();
        passiveList[passiveAbility.WeaponPassiveAbilityNo - 1] = 1;
        entity.UnlockWeaponPassiveAbilityNoList = passiveList;

        await this.apiContext.PlayerPassiveAbilities.AddAsync(
            new()
            {
                DeviceAccountId = this.playerDetailsService.AccountId,
                WeaponPassiveAbilityId = passiveAbility.Id
            }
        );
    }
}
