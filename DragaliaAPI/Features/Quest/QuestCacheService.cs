using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.Quest;

public class QuestCacheService(
    IDistributedCache distributedCache,
    IPlayerIdentityService playerIdentityService,
    ILogger<QuestCacheService> logger
) : IQuestCacheService
{
    private static readonly DistributedCacheEntryOptions QuestEntryCacheOptions =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) };

    public async Task SetQuestGroupQuestIdAsync(int questGroupId, int questId)
    {
        await distributedCache.SetStringAsync(
            Schema.QuestEventId(playerIdentityService.AccountId, questGroupId),
            questId.ToString(),
            QuestEntryCacheOptions
        );
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

        logger.LogError(
            "Tried to parse cached quest id string {questIdString} but it was not an int",
            questIdString
        );

        return null;
    }

    public async Task RemoveQuestGroupQuestIdAsync(int questGroupId)
    {
        await distributedCache.RemoveAsync(
            Schema.QuestEventId(playerIdentityService.AccountId, questGroupId)
        );
    }
}

file static class Schema
{
    public static string QuestEventId(string playerId, int questEventId) =>
        $":{playerId}:lastcompletedquest:{questEventId}";
}
