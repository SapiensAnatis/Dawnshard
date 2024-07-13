using DragaliaAPI.Database;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V23Update(
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
            logger.LogInformation("PlayerDiamondData not found: adding new row");
            apiContext.PlayerDiamondData.Add(new() { ViewerId = playerIdentityService.ViewerId, });
        }
    }
}
