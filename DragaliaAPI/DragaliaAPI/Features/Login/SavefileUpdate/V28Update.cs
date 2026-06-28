using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using LinqToDB;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Fixes missing dragon stories for dragons whose default reliability level is >= 5 (e.g. Arsene).
/// </summary>
public partial class V28Update(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V28Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 28;

    public async Task Apply()
    {
        var dragonsAtOrOver5Bond = await apiContext
            .PlayerDragonReliability.Where(x => x.Level >= 5)
            .Select(x => new { x.DragonId, x.Level })
            .ToListAsync();

        List<DbPlayerStoryState> intendedStoryStates = new(dragonsAtOrOver5Bond.Count * 2);

        foreach (var info in dragonsAtOrOver5Bond)
        {
            if (!MasterAsset.DragonStories.TryGetValue((int)info.DragonId, out StoryData? data))
            {
                continue;
            }

            int storiesToUnlock = info.Level >= 15 ? Math.Min(2, data.StoryIds.Length) : 1;

            for (int i = 0; i < storiesToUnlock; i++)
            {
                intendedStoryStates.Add(
                    new DbPlayerStoryState()
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        StoryId = data.StoryIds[i],
                        State = StoryState.Unlocked,
                        StoryType = StoryTypes.Dragon,
                    }
                );
            }
        }

        int rowsAffected = await apiContext
            .PlayerStoryState.Merge()
            .Using(intendedStoryStates)
            .OnTargetKey()
            .InsertWhenNotMatched()
            .MergeAsync();

        Log.AddedNewStories(logger, rowsAffected);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "V28Update added {Count} missing dragon stories")]
        public static partial void AddedNewStories(ILogger logger, int count);
    }
}
