using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

public partial class V24Update(
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
            Log.PlayerHelperNotFoundAddingNewRow(logger);
            apiContext.PlayerHelpers.Add(
                new() { ViewerId = playerIdentityService.ViewerId, CharaId = Charas.ThePrince }
            );
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "PlayerHelper not found: adding new row")]
        public static partial void PlayerHelperNotFoundAddingNewRow(ILogger logger);
    }
}
