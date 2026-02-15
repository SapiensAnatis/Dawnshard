using System.Diagnostics;
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
/// Fixes missing dragon stories due to a bug where levelling up to >=15 bond in one request
/// would only grant one story.
/// </summary>
public partial class V25Update(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V25Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 25;

    public async Task Apply()
    {
        List<DragonId> dragonsAtOrOver15Bond = await apiContext
            .PlayerDragonReliability.Where(x => x.Level >= 15)
            .Select(x => x.DragonId)
            .ToListAsync();

        List<DbPlayerStoryState> intendedStoryStates = new(dragonsAtOrOver15Bond.Count * 2);

        foreach (DragonId dragonId in dragonsAtOrOver15Bond)
        {
            // Handle invalid dragon IDs. Shouldn't really be possible, but people do all sorts of shit with save
            // editing, and a crashing save update blocks login so let's be cautious
            if (MasterAsset.DragonStories.TryGetValue((int)dragonId, out StoryData? data))
            {
                intendedStoryStates.AddRange(
                    data.StoryIds.Select(x => new DbPlayerStoryState()
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        StoryId = x,
                        State = StoryState.Unlocked,
                        StoryType = StoryTypes.Dragon,
                    })
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
        [LoggerMessage(LogLevel.Information, "V25Update added {Count} missing dragon stories")]
        public static partial void AddedNewStories(ILogger logger, int count);
    }
}
