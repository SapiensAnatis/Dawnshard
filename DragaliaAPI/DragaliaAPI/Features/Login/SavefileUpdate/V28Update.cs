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
        List<int> arseneLevels = await apiContext
            .PlayerDragonReliability.Where(x => x.DragonId == DragonId.Arsene && x.Level >= 5)
            .Select(x => x.Level)
            .ToListAsync();

        if (
            arseneLevels.Count == 0
            || !MasterAsset.DragonStories.TryGetValue((int)DragonId.Arsene, out StoryData? data)
        )
        {
            Log.AddedNewStories(logger, 0);
            return;
        }

        List<DbPlayerStoryState> intendedStoryStates = new(arseneLevels.Count * 2);

        foreach (int level in arseneLevels)
        {
            int storiesToUnlock = level >= 15 ? 2 : 1;

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
