using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DragaliaAPI.Services;

public class DungeonService : IDungeonService
{
    private readonly IDistributedCache cache;
    private readonly IOptionsMonitor<RedisOptions> options;
    private DistributedCacheEntryOptions CacheOptions =>
        new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(options.CurrentValue.DungeonExpiryTimeMinutes)
        };

    private static class Schema
    {
        public static string DungeonKey_DungeonData(string dungeonKey)
        {
            return $":dungeon:{dungeonKey}";
        }
    }

    public DungeonService(IDistributedCache cache, IOptionsMonitor<RedisOptions> options)
    {
        this.cache = cache;
        this.options = options;
    }

    public async Task<string> StartDungeon(DungeonSession dungeonSession)
    {
        string key = Guid.NewGuid().ToString();
        await cache.SetStringAsync(key, JsonSerializer.Serialize(dungeonSession), CacheOptions);
        return key;
    }

    public async Task<DungeonSession> GetDungeon(string dungeonKey)
    {
        string json =
            await cache.GetStringAsync(dungeonKey) ?? throw new DungeonException(dungeonKey);

        return JsonSerializer.Deserialize<DungeonSession>(json)
            ?? throw new JsonException("Could not deserialize dungeon session.");
    }

    public async Task<DungeonSession> FinishDungeon(string dungeonKey)
    {
        DungeonSession session = await this.GetDungeon(dungeonKey);

        await cache.RemoveAsync(dungeonKey);

        return session;
    }
}
