using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonService(
    IDistributedCache cache,
    IOptionsMonitor<RedisCachingOptions> options,
    IPlayerIdentityService playerIdentityService,
    ILogger<DungeonService> logger
) : IDungeonService
{
    private DungeonSession? currentSession;
    private string? currentKey;

    private DistributedCacheEntryOptions CacheOptions =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.CurrentValue.DungeonExpiryTimeMinutes)
        };

    private static class Schema
    {
        public static string DungeonKey_DungeonData(long viewerId, string dungeonKey) =>
            $":dungeon:{viewerId}:{dungeonKey}";
    }

    public string CreateSession(DungeonSession dungeonSession)
    {
        this.currentKey = Guid.NewGuid().ToString();
        this.currentSession = dungeonSession;

        logger.LogDebug("Created dungeon session with key {Key}", currentKey);

        return currentKey;
    }

    public async Task<DungeonSession> GetSession(
        string dungeonKey,
        CancellationToken cancellationToken
    )
    {
        DungeonSession session =
            await cache.GetJsonAsync<DungeonSession>(
                Schema.DungeonKey_DungeonData(playerIdentityService.ViewerId, dungeonKey),
                cancellationToken
            ) ?? throw new DungeonException(dungeonKey);

        this.currentSession = session;
        this.currentKey = dungeonKey;

        return session;
    }

    public async Task SaveSession(CancellationToken cancellationToken)
    {
        if (this.currentSession == null || this.currentKey == null)
        {
            throw new InvalidOperationException(
                "Cannot perform WriteSession when no dungeon session is being tracked. "
                    + "A session must be loaded either via CreateSession or GetSession before one can be saved."
            );
        }

        await cache.SetJsonAsync(
            Schema.DungeonKey_DungeonData(playerIdentityService.ViewerId, this.currentKey),
            this.currentSession,
            CacheOptions,
            cancellationToken
        );
    }

    public async Task ModifySession(
        string dungeonKey,
        Action<DungeonSession> update,
        CancellationToken cancellationToken
    )
    {
        this.currentSession ??= await this.GetSession(dungeonKey, cancellationToken);
        update.Invoke(this.currentSession);
    }

    public async Task RemoveSession(string dungeonKey, CancellationToken cancellationToken)
    {
        logger.LogDebug("Removing dungeon session with key {Key}", dungeonKey);

        await cache.RemoveAsync(
            Schema.DungeonKey_DungeonData(playerIdentityService.ViewerId, dungeonKey),
            cancellationToken
        );

        this.currentSession = null;
        this.currentKey = null;
    }
}
