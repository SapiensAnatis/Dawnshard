using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonService : IDungeonService
{
    private readonly IDistributedCache cache;
    private readonly IOptionsMonitor<RedisOptions> options;
    private readonly ILogger<DungeonService> logger;

    private DistributedCacheEntryOptions CacheOptions =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.CurrentValue.DungeonExpiryTimeMinutes)
        };

    private static class Schema
    {
        public static string DungeonKey_DungeonData(string dungeonKey) => $":dungeon:{dungeonKey}";
    }

    public DungeonService(
        IDistributedCache cache,
        IOptionsMonitor<RedisOptions> options,
        ILogger<DungeonService> logger
    )
    {
        this.cache = cache;
        this.options = options;
        this.logger = logger;
    }

    public async Task<string> StartDungeon(DungeonSession dungeonSession)
    {
        string dungeonKey = Guid.NewGuid().ToString();
        await WriteDungeon(dungeonKey, dungeonSession);

        this.logger.LogDebug("Issued dungeon key {key}", dungeonKey);

        return dungeonKey;
    }

    public async Task<DungeonSession> GetDungeon(string dungeonKey)
    {
        string json =
            await cache.GetStringAsync(Schema.DungeonKey_DungeonData(dungeonKey))
            ?? throw new DungeonException(dungeonKey);

        return JsonSerializer.Deserialize<DungeonSession>(json)
            ?? throw new JsonException("Could not deserialize dungeon session.");
    }

    private async Task WriteDungeon(string dungeonKey, DungeonSession session)
    {
        await cache.SetStringAsync(
            Schema.DungeonKey_DungeonData(dungeonKey),
            JsonSerializer.Serialize(session),
            CacheOptions
        );
    }

    public async Task AddEnemies(
        string dungeonKey,
        int areaIndex,
        IEnumerable<AtgenEnemy> enemyList
    )
    {
        DungeonSession session = await this.GetDungeon(dungeonKey);

        if (session.EnemyList.ContainsKey(areaIndex))
            this.logger.LogWarning("The drops for area index {idx} will be overwritten", areaIndex);

        session.EnemyList[areaIndex] = enemyList;

        await WriteDungeon(dungeonKey, session);
    }

    public async Task SetIsHost(string dungeonKey, bool isHost)
    {
        DungeonSession session = await this.GetDungeon(dungeonKey);

        session.IsHost = isHost;

        await WriteDungeon(dungeonKey, session);
    }

    public async Task<DungeonSession> FinishDungeon(string dungeonKey)
    {
        DungeonSession session = await GetDungeon(dungeonKey);

        this.logger.LogDebug("Completed dungeon with key {key}", dungeonKey);

        // Don't remove in case the client re-calls due to timeout
        // await cache.RemoveAsync(Schema.DungeonKey_DungeonData(dungeonKey));

        return session;
    }
}
