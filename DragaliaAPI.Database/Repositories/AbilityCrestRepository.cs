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

    public async Task Add(AbilityCrests abilityCrestId)
    {
        this.logger.LogDebug("Adding ability crest {print}", abilityCrestId);

        if (await this.FindAsync(abilityCrestId) is not null)
        {
            this.logger.LogWarning("Ability crest was already owned.");
            return;
        }

        await this.apiContext.PlayerAbilityCrests.AddAsync(
            new DbAbilityCrest(this.playerIdentityService.AccountId, abilityCrestId)
        );
    }

    public async Task<DbAbilityCrest?> FindAsync(AbilityCrests abilityCrestId) =>
        await this.apiContext.PlayerAbilityCrests.FindAsync(
            this.playerIdentityService.AccountId,
            abilityCrestId
        );

    public async Task AddOrUpdateSet(DbAbilityCrestSet abilityCrestSet)
    {
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
