using System.Diagnostics;
using DragaliaAPI.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DragaliaAPI.Infrastructure.Authentication;

internal static partial class GameJwtAuthenticationCallbacks
{
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        // Use ID-TOKEN from header rather than id_token in body - the header is updated
        // on refreshes and generally seems to be the more accurate source of truth
        if (context.Request.Headers.TryGetValue("ID-TOKEN", out StringValues idToken))
        {
            context.Token = idToken.FirstOrDefault();
        }

        return Task.CompletedTask;
    }

    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        JsonWebToken jsonWebToken = (JsonWebToken)context.SecurityToken;

        ApiContext apiContext =
            context.HttpContext.RequestServices.GetRequiredService<ApiContext>();

        ILogger logger = context
            .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(GameJwtAuthenticationCallbacks));

        var playerInfo = await apiContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.AccountId == jsonWebToken.Subject)
            .Select(x => new { x.ViewerId, x.UserData!.Name })
            .FirstOrDefaultAsync();

        Log.RetrievedPlayerInfo(logger, jsonWebToken.Subject, playerInfo);

        if (playerInfo is not null)
        {
            context.Principal?.InitializeDawnshardIdentity(
                jsonWebToken.Subject,
                playerInfo.ViewerId,
                playerInfo.Name
            );
        }
    }

    public static Task OnChallenge(JwtBearerChallengeContext context)
    {
        return Task.CompletedTask;
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
