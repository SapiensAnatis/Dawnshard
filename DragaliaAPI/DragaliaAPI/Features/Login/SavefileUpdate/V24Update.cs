using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

public class V24Update(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V24Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 24;

    public async Task Apply()
    {
        if (!await apiContext.PlayerHelpers.AnyAsync())
        {
            logger.LogInformation("PlayerHelper not found: adding new row");
            apiContext.PlayerHelpers.Add(
                new() { ViewerId = playerIdentityService.ViewerId, CharaId = Charas.ThePrince }
            );
        }
    }
}
