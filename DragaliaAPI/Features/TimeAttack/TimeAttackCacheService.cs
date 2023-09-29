using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.TimeAttack;

public class TimeAttackCacheService(
    IDistributedCache cache,
    IPlayerIdentityService playerIdentityService,
    ILogger<TimeAttackCacheService> logger
) : ITimeAttackCacheService
{
    public async Task Set(int questId, PartyInfo partyInfo)
    {
        logger.LogDebug("Setting time attack cache entry for quest id {id}", questId);

        TimeAttackCacheEntry entry = new(questId, partyInfo);

        await cache.SetStringAsync(this.Key, JsonSerializer.Serialize(entry));
    }

    public async Task<TimeAttackCacheEntry?> Get()
    {
        logger.LogDebug("Getting time attack cache entry");

        string? json = await cache.GetStringAsync(this.Key);

        if (json is null)
            return null;

        return JsonSerializer.Deserialize<TimeAttackCacheEntry>(json);
    }

    private string Key => $":timeattack:{playerIdentityService.AccountId}";
}
