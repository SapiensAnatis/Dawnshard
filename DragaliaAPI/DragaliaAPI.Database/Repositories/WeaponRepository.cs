using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeaponBodiesEnum = DragaliaAPI.Shared.Definitions.Enums.WeaponBodies;

namespace DragaliaAPI.Database.Repositories;

public partial class WeaponRepository : IWeaponRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<WeaponRepository> logger;

    private static readonly ImmutableArray<int> AstralsBaneAbilityIds = ImmutableArray.Create(
        595,
        596,
        597,
        598,
        599
    );

    public WeaponRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<WeaponRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbWeaponBody> WeaponBodies =>
        this.apiContext.PlayerWeapons.Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbWeaponSkin> WeaponSkins =>
        this.apiContext.PlayerWeaponSkins.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbWeaponPassiveAbility> WeaponPassiveAbilities =>
        this.apiContext.PlayerPassiveAbilities.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
        );

    public IQueryable<DbWeaponPassiveAbility> GetPassiveAbilities(Charas id)
    {
        CharaData charaData = MasterAsset.CharaData.Get(id);

        IEnumerable<int> searchIds = MasterAsset
            .WeaponPassiveAbility.Enumerable.Where(x =>
                x.WeaponType == charaData.WeaponType && x.ElementalType == charaData.ElementalType
            )
            .ExceptBy(AstralsBaneAbilityIds, x => x.AbilityId) // Sending astral abilities in the list breaks scorch res. Don't ask me why.
            .Select(x => x.Id);

        return this.apiContext.PlayerPassiveAbilities.Where(x =>
            x.ViewerId == this.playerIdentityService.ViewerId
            && searchIds.Contains(x.WeaponPassiveAbilityId)
        );
    }

    public async Task Add(WeaponBodies weaponBodyId)
    {
        Log.AddingWeapon(this.logger, weaponBodyId);

        await this.apiContext.PlayerWeapons.AddAsync(
            new() { ViewerId = this.playerIdentityService.ViewerId, WeaponBodyId = weaponBodyId }
        );
    }

    public async Task AddSkin(int weaponSkinId)
    {
        Log.AddingWeaponSkin(this.logger, weaponSkinId);

        if (
            await this.apiContext.PlayerWeaponSkins.FindAsync(
                this.playerIdentityService.ViewerId,
                weaponSkinId
            )
            is not null
        )
        {
            Log.WeaponSkinWasAlreadyOwned(this.logger);
            return;
        }

        await this.apiContext.PlayerWeaponSkins.AddAsync(
            new()
            {
                ViewerId = this.playerIdentityService.ViewerId,
                WeaponSkinId = weaponSkinId,
                GetTime = DateTimeOffset.UtcNow,
            }
        );
    }

    public async Task<bool> CheckOwnsWeapons(params WeaponBodies[] weaponIds)
    {
        List<WeaponBodies> filtered = weaponIds.Where(x => x != WeaponBodiesEnum.Empty).ToList();

        return await this
                .WeaponBodies.Select(x => x.WeaponBodyId)
                .Where(x => filtered.Contains(x))
                .CountAsync() == filtered.Count;
    }

    public async Task<DbWeaponBody?> FindAsync(WeaponBodies id) =>
        await this.apiContext.PlayerWeapons.FindAsync(this.playerIdentityService.ViewerId, id);

    public async Task AddPassiveAbility(WeaponBodies id, WeaponPassiveAbility passiveAbility)
    {
        Log.UnlockingPassiveAbility(this.logger, passiveAbility);

        DbWeaponBody? entity = await this.FindAsync(id);
        ArgumentNullException.ThrowIfNull(entity);

        entity.UnlockWeaponPassiveAbilityNoList[passiveAbility.WeaponPassiveAbilityNo - 1] = 1;

        if (
            await this.WeaponPassiveAbilities.AnyAsync(x =>
                x.WeaponPassiveAbilityId == passiveAbility.Id
            )
        )
        {
            Log.PassiveWasAlreadyOwned(this.logger);
            return;
        }

        await this.apiContext.PlayerPassiveAbilities.AddAsync(
            new()
            {
                ViewerId = this.playerIdentityService.ViewerId,
                WeaponPassiveAbilityId = passiveAbility.Id,
            }
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Adding weapon {weapon}")]
        public static partial void AddingWeapon(ILogger logger, WeaponBodies weapon);

        [LoggerMessage(LogLevel.Debug, "Adding weapon skin {skin}")]
        public static partial void AddingWeaponSkin(ILogger logger, int skin);

        [LoggerMessage(LogLevel.Debug, "Weapon skin was already owned.")]
        public static partial void WeaponSkinWasAlreadyOwned(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Unlocking passive ability {@ability}")]
        public static partial void UnlockingPassiveAbility(
            ILogger logger,
            WeaponPassiveAbility ability
        );

        [LoggerMessage(LogLevel.Debug, "Passive was already owned.")]
        public static partial void PassiveWasAlreadyOwned(ILogger logger);
    }
}
