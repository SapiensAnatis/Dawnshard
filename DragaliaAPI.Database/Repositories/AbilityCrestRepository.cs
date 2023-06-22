using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

public class AbilityCrestRepository : IAbilityCrestRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<AbilityCrestRepository> logger;

    public AbilityCrestRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<AbilityCrestRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public IQueryable<DbAbilityCrestSet> AbilityCrestSets =>
        this.apiContext.PlayerAbilityCrestSets.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task Add(
        AbilityCrests abilityCrestId,
        int? limitBreakCount = null,
        int? buildupCount = null,
        int? equipableCount = null
    )
    {
        this.logger.LogDebug("Adding ability crest {print}", abilityCrestId);

        if (await this.FindAsync(abilityCrestId) is not null)
        {
            this.logger.LogWarning("Ability crest was already owned.");
            return;
        }

        DbAbilityCrest entity = new DbAbilityCrest(
            this.playerIdentityService.AccountId,
            abilityCrestId
        );

        if (limitBreakCount is not null)
            entity.LimitBreakCount = limitBreakCount.Value;
        if (buildupCount is not null)
            entity.BuildupCount = buildupCount.Value;
        if (equipableCount is not null)
            entity.EquipableCount = equipableCount.Value;

        this.apiContext.PlayerAbilityCrests.Add(entity);
    }

    public async Task<DbAbilityCrest?> FindAsync(AbilityCrests abilityCrestId) =>
        await this.apiContext.PlayerAbilityCrests.FindAsync(
            this.playerIdentityService.AccountId,
            abilityCrestId
        );

    public async Task AddOrUpdateSet(DbAbilityCrestSet abilityCrestSet)
    {
        abilityCrestSet.DeviceAccountId = this.playerIdentityService.AccountId;
        DbAbilityCrestSet? dbAbilityCrestSet = await this.FindSetAsync(
            abilityCrestSet.AbilityCrestSetNo
        );

        if (dbAbilityCrestSet is null)
        {
            await this.apiContext.PlayerAbilityCrestSets.AddAsync(abilityCrestSet);
        }
        else
        {
            this.apiContext.PlayerAbilityCrestSets
                .Entry(dbAbilityCrestSet)
                .CurrentValues.SetValues(abilityCrestSet);
        }
    }

    public async Task<DbAbilityCrestSet?> FindSetAsync(int abilityCrestSetNo) =>
        await this.apiContext.PlayerAbilityCrestSets.FindAsync(
            this.playerIdentityService.AccountId,
            abilityCrestSetNo
        );
}
