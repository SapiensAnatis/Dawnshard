using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.TimeAttack;

public partial class TimeAttackCacheService(
    IDistributedCache cache,
    IPlayerIdentityService playerIdentityService,
    ILogger<TimeAttackCacheService> logger
) : ITimeAttackCacheService
{
    public async Task Set(int questId, PartyInfo partyInfo)
    {
        Log.SettingTimeAttackCacheEntryForQuestId(logger, questId);

        TimeAttackCacheEntry entry = new(questId, partyInfo);

        await cache.SetStringAsync(this.Key, JsonSerializer.Serialize(entry));
    }

    public async Task<TimeAttackCacheEntry?> Get()
    {
        Log.GettingTimeAttackCacheEntry(logger);

        string? json = await cache.GetStringAsync(this.Key);

        if (json is null)
            return null;

        return JsonSerializer.Deserialize<TimeAttackCacheEntry>(json);
    }

    private string Key => $":timeattack:{playerIdentityService.AccountId}";

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Setting time attack cache entry for quest id {id}")]
        public static partial void SettingTimeAttackCacheEntryForQuestId(ILogger logger, int id);
        [LoggerMessage(LogLevel.Debug, "Getting time attack cache entry")]
        public static partial void GettingTimeAttackCacheEntry(ILogger logger);
    }
}
