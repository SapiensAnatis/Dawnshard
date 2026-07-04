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
/// Fixes missing dragon stories for Arsene, whose default reliability level of 30 meant that
/// stories were not unlocked on receipt (unlike level-up, which is handled by <see cref="V25Update"/>).
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
        DbPlayerDragonReliability? arsene =
            await apiContext.PlayerDragonReliability.FirstOrDefaultAsync(x =>
                x.DragonId == DragonId.Arsene
            );

        if (
            arsene is null
            || !MasterAsset.DragonStories.TryGetValue((int)DragonId.Arsene, out StoryData? data)
        )
        {
            Log.AddedNewStories(logger, 0);
            return;
        }

        // Arsene's default reliability level is 30, so any existing reliability entry implies
        // both stories should be unlocked -- there is no need to check the level.
        List<DbPlayerStoryState> intendedStoryStates = data
            .StoryIds.Select(storyId => new DbPlayerStoryState()
            {
                ViewerId = playerIdentityService.ViewerId,
                StoryId = storyId,
                State = StoryState.Unlocked,
                StoryType = StoryTypes.Dragon,
            })
            .ToList();

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
