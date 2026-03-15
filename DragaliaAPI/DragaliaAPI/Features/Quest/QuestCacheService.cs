using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.Quest;

public partial class QuestCacheService(
    IDistributedCache distributedCache,
    IPlayerIdentityService playerIdentityService,
    ILogger<QuestCacheService> logger
) : IQuestCacheService
{
    public async Task SetQuestGroupQuestIdAsync(int questGroupId, int questId)
    {
        string key = Schema.QuestEventId(playerIdentityService.AccountId, questGroupId);
        Log.SettingQuestIdKey(logger, key);

        await distributedCache.SetStringAsync(key, questId.ToString());
    }

    public async Task<int?> GetQuestGroupQuestIdAsync(int questGroupId)
    {
        string? questIdString = await distributedCache.GetStringAsync(
            Schema.QuestEventId(playerIdentityService.AccountId, questGroupId)
        );

        if (int.TryParse(questIdString, out int questId))
        {
            return questId;
        }

        Log.TriedToParseCachedQuestIdStringButItWasNotAnInt(logger, questIdString);

        return null;
    }

    public async Task RemoveQuestGroupQuestIdAsync(int questGroupId)
    {
        await distributedCache.RemoveAsync(
            Schema.QuestEventId(playerIdentityService.AccountId, questGroupId)
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Setting quest id key: {key}")]
        public static partial void SettingQuestIdKey(ILogger logger, string key);
        [LoggerMessage(LogLevel.Error, "Tried to parse cached quest id string {questIdString} but it was not an int")]
        public static partial void TriedToParseCachedQuestIdStringButItWasNotAnInt(ILogger logger, string? questIdString);
    }
}

file static class Schema
{
    public static string QuestEventId(string playerId, int questEventId) =>
        $":{playerId}:lastcompletedquest:{questEventId}";
}
