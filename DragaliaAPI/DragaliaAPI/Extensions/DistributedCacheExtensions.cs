using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<TObject?> GetJsonAsync<TObject>(
        this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken = default
    )
        where TObject : class
    {
        string? json = await cache.GetStringAsync(key, cancellationToken);
        if (json == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TObject>(json);
    }

    public static Task SetJsonAsync(
        this IDistributedCache cache,
        string key,
        object entry,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default
    ) => cache.SetStringAsync(key, JsonSerializer.Serialize(entry), options, cancellationToken);
}
