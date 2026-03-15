using DragaliaAPI.Database;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

public partial class V23Update(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V23Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 23;

    public async Task Apply()
    {
        if (!await apiContext.PlayerDiamondData.AnyAsync())
        {
            Log.PlayerDiamondDataNotFoundAddingNewRow(logger);
            apiContext.PlayerDiamondData.Add(new() { ViewerId = playerIdentityService.ViewerId });
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "PlayerDiamondData not found: adding new row")]
        public static partial void PlayerDiamondDataNotFoundAddingNewRow(ILogger logger);
    }
}
