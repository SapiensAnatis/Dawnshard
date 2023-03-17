using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

internal class AbilityCrestRepository : IAbilityCrestRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerDetailsService playerDetailsService;
    private readonly ILogger<AbilityCrestRepository> logger;

    public AbilityCrestRepository(
        ApiContext apiContext,
        IPlayerDetailsService playerDetailsService,
        ILogger<AbilityCrestRepository> logger
    )
    {
        this.apiContext = apiContext;
        this.playerDetailsService = playerDetailsService;
        this.logger = logger;
    }

    public IQueryable<DbAbilityCrest> AbilityCrests =>
        this.apiContext.PlayerAbilityCrests.Where(
            x => x.DeviceAccountId == this.playerDetailsService.AccountId
        );

    public async Task Add(AbilityCrests abilityCrestId)
    {
        this.logger.LogDebug("Adding ability crest {print}", abilityCrestId);

        if (await this.FindAsync(abilityCrestId) is not null)
        {
            this.logger.LogDebug("Ability crest was already owned.");
            return;
        }

        await this.apiContext.PlayerAbilityCrests.AddAsync(
            new DbAbilityCrest(this.playerDetailsService.AccountId, abilityCrestId)
        );
    }

    public async Task<DbAbilityCrest?> FindAsync(AbilityCrests abilityCrestId) =>
        await this.apiContext.PlayerAbilityCrests.FindAsync(
            this.playerDetailsService.AccountId,
            abilityCrestId
        );
}
