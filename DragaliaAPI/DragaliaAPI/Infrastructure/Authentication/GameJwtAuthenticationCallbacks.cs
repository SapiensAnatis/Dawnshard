using DragaliaAPI.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Infrastructure.Authentication;

internal static partial class GameJwtAuthenticationCallbacks
{
    private static readonly DistributedCacheEntryOptions CacheOptions =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        // Use ID-TOKEN from header rather than id_token in body - the header is updated
        // on refreshes and generally seems to be the more accurate source of truth
        if (context.Request.Headers.TryGetValue(DragaliaHttpConstants.Headers.IdToken, out StringValues idToken))
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

    public static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        string? deviceId = null;
        if (context.Request.Headers.TryGetValue("DeviceId", out StringValues values))
        {
            deviceId = values.FirstOrDefault();
        }

        if (context.AuthenticateFailure is SecurityTokenExpiredException && deviceId is not null)
        {
            await WriteExpiredIdTokenResponse(context, deviceId);
        }
        else
        {
            await context.HttpContext.WriteResultCodeResponse(ResultCode.IdTokenError);
        }
    }

    private static async Task WriteExpiredIdTokenResponse(
        JwtBearerChallengeContext context,
        string deviceId
    )
    {
        ILogger logger = context
            .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(GameJwtAuthenticationCallbacks));

        IDistributedCache cache =
            context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        Log.IdTokenExpired(
            logger,
            (context.AuthenticateFailure as SecurityTokenExpiredException)?.Expires
        );

        /*
         * Setting the below header should cause the client to route back to Nintendo login.
         * However, sometimes the client returns with the same invalid ID token. We track who
         * we've sent refresh requests to, and if they come back here again we forcibly remove
         * them to the title screen to get a new token.
         */

        string redisKey = $"refresh_sent:{deviceId}";

        if (await cache.GetStringAsync(redisKey) is not null)
        {
            Log.DetectedRepeatedTokenExpiry(logger);
            await context.HttpContext.WriteResultCodeResponse(ResultCode.CommonAuthError);
            return;
        }

        await cache.SetStringAsync(redisKey, "true", CacheOptions);

        Log.IssuingIdTokenRefreshRequest(logger);

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.Headers.Append("Is-Required-Refresh-Id-Token", "true");
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "PlayerInfo for game account {AccountId}: {@PlayerInfo}")]
        public static partial void RetrievedPlayerInfo(
            ILogger logger,
            string accountId,
            object? playerInfo
        );

        [LoggerMessage(LogLevel.Error, "Detected repeated SecurityTokenExpiredException.")]
        public static partial void DetectedRepeatedTokenExpiry(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "ID token was expired. Expiry: {Expiry}")]
        public static partial void IdTokenExpired(ILogger logger, DateTime? expiry);

        [LoggerMessage(LogLevel.Debug, "Issuing ID token refresh request.")]
        public static partial void IssuingIdTokenRefreshRequest(ILogger logger);
    }
}
