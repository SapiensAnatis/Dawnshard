using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Fixes dragons that have no dragon reliability entry.
/// </summary>
[UsedImplicitly]
public class V17Update(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V17Update> logger
) : ISavefileUpdate
{
    private readonly ApiContext apiContext = apiContext;
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;
    private readonly ILogger<V17Update> logger = logger;

    public int SavefileVersion => 17;

    public async Task Apply()
    {
        IAsyncEnumerable<Dragons> missingReliabilities = this
            .apiContext.PlayerDragonData.Where(dragon =>
                dragon.ViewerId == this.playerIdentityService.ViewerId
            )
            .Where(dragon =>
                !this.apiContext.PlayerDragonReliability.Any(rel =>
                    rel.ViewerId == this.playerIdentityService.ViewerId
                    && rel.DragonId == dragon.DragonId
                )
            )
            .Select(x => x.DragonId)
            .Distinct()
            .AsAsyncEnumerable();

        await foreach (Dragons missingReliability in missingReliabilities)
        {
            this.logger.LogDebug(
                "Adding reliability entry for {missingReliability}",
                missingReliability
            );

            this.apiContext.PlayerDragonReliability.Add(
                DbPlayerDragonReliabilityFactory.Create(
                    this.playerIdentityService.ViewerId,
                    missingReliability
                )
            );
        }
    }
}
