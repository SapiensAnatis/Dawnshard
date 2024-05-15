using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.SavefileUpdate;

[UsedImplicitly]
public partial class V21Update(
    IWallService wallService,
    IPlayerIdentityService playerIdentityService,
    ILogger<V21Update> logger,
    ApiContext apiContext
) : ISavefileUpdate
{
    public int SavefileVersion => 21;

    public async Task Apply()
    {
        if (
            await wallService.CheckWallInitialized()
            && await apiContext.WallRewardDates.FindAsync(playerIdentityService.ViewerId) is null
        )
        {
            Log.AddingRewardDate(logger);

            apiContext.WallRewardDates.Add(
                new DbWallRewardDate()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    LastClaimDate = DateTimeOffset.UnixEpoch, // Allow immediately claiming the reward.
                }
            );
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Adding WallRewardDate entry")]
        public static partial void AddingRewardDate(ILogger logger);
    }
}
