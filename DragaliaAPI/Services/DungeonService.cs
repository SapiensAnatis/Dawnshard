using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DragaliaAPI.Services;

public class DungeonService : IDungeonService
{
    private readonly IDistributedCache cache;
    private readonly DistributedCacheEntryOptions cacheOptions;

    private static class Schema
    {
        public static string DungeonKey_DungeonData(string dungeonKey)
        {
            return $":dungeon:{dungeonKey}";
        }
    }

    public DungeonService(IDistributedCache cache, IConfiguration configuration)
    {
        int expiryTimeMinutes = configuration.GetValue<int>("DungeonExpiryTimeMinutes");
        this.cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryTimeMinutes)
        };

        this.cache = cache;
    }

    public async Task<string> StartDungeon(DungeonSession dungeonSession)
    {
        string key = Guid.NewGuid().ToString();
        await cache.SetStringAsync(key, JsonSerializer.Serialize(dungeonSession), cacheOptions);
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
