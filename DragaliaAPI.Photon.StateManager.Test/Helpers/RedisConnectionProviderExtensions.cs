using DragaliaAPI.Photon.StateManager.Models;
using Redis.OM;
using Redis.OM.Contracts;

namespace DragaliaAPI.Photon.StateManager.Test.Helpers;

public static class RedisConnectionProviderExtensions
{
    public static async Task<RedisGame?> GetGame(
        this IRedisConnectionProvider connectionProvider,
        string gameName
    )
    {
        // Go directly to Redis and get the key, as connectionProvider.RedisCollection<T>
        // seems to employ some kind of client-side caching.
        return await connectionProvider.Connection.JsonGetAsync<RedisGame>(
            $"{typeof(RedisGame).FullName}:{gameName}"
        );
    }
}
