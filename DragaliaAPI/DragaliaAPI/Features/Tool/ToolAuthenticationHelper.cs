using System.Diagnostics;
using System.Security.Claims;
using DragaliaAPI.Database;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DragaliaAPI.Features.Tool;

internal static partial class ToolAuthenticationHelper
{
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        // Use ID-TOKEN from header rather than /tool/auth body - the header is updated
        // on refreshes and generally seems to be the more accurate source of truth
        if (context.Request.Headers.TryGetValue("ID-TOKEN", out StringValues idToken))
        {
            context.Token = idToken.FirstOrDefault();
        }

        return Task.CompletedTask;
    }

    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.SecurityToken is not JsonWebToken jsonWebToken)
        {
            throw new UnreachableException(
                "TokenValidatedContext.SecurityToken was not a JsonWebToken"
            );
        }

        ApiContext apiContext =
            context.HttpContext.RequestServices.GetRequiredService<ApiContext>();

        ILogger logger = context
            .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(ToolAuthenticationHelper));

        var playerInfo = await apiContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.AccountId == jsonWebToken.Subject)
            .Select(x => new { x.ViewerId, x.UserData!.Name })
            .FirstOrDefaultAsync();

        Log.RetrievedPlayerInfo(logger, jsonWebToken.Subject, playerInfo);

        if (playerInfo is not null)
        {
            ClaimsIdentity playerIdentity =
                new(
                    [
                        new Claim(CustomClaimType.AccountId, jsonWebToken.Subject),
                        new Claim(CustomClaimType.ViewerId, playerInfo.ViewerId.ToString()),
                        new Claim(CustomClaimType.PlayerName, playerInfo.Name),
                    ]
                )
                {
                    Label = AuthConstants.IdentityLabels.Dawnshard,
                };

            context.Principal?.AddIdentity(playerIdentity);
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "PlayerInfo for game account {AccountId}: {@PlayerInfo}")]
        public static partial void RetrievedPlayerInfo(
            ILogger logger,
            string accountId,
            object? playerInfo
        );
    }
}
