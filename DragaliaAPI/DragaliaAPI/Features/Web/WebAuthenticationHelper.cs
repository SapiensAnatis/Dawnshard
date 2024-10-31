using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using DragaliaAPI.Database;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DragaliaAPI.Features.Web;

public static class WebAuthenticationHelper
{
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        if (context.Request.Cookies.TryGetValue("idToken", out string? idToken))
        {
            context.Token = idToken;
        }

        if (
            AuthenticationHeaderValue.TryParse(
                context.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authHeader
            ) && authHeader is { Scheme: "Bearer", Parameter: not null }
        )
        {
            context.Token = authHeader.Parameter;
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

        ILogger logger = context
            .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(WebAuthenticationHelper));

        PlayerInfo? playerInfo = await GetPlayerInfo(context, jsonWebToken, logger);

        if (playerInfo is not null)
        {
            context.Principal?.InitializeDawnshardIdentity(
                playerInfo.AccountId,
                playerInfo.ViewerId,
                playerInfo.Name
            );
        }
    }

    private static async Task<PlayerInfo?> GetPlayerInfo(
        TokenValidatedContext context,
        JsonWebToken jwt,
        ILogger logger
    )
    {
        // TODO: Rewrite using HybridCache when .NET 9 releases
        IDistributedCache cache =
            context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        string cacheKey = $":playerinfo:${jwt.Subject}";

        if (await cache.GetJsonAsync<PlayerInfo>(cacheKey) is { } cachedPlayerInfo)
        {
            logger.LogDebug("Using cached player info: {@PlayerInfo}", cachedPlayerInfo);
            return cachedPlayerInfo;
        }

        IBaasApi baasApi = context.HttpContext.RequestServices.GetRequiredService<IBaasApi>();
        string? gameAccountId = await baasApi.GetUserId(jwt.EncodedToken);

        logger.LogDebug(
            "Retrieved game account {GameAccountId} from BaaS for web account {WebAccountId}",
            gameAccountId,
            jwt.Subject
        );

        if (gameAccountId is null)
        {
            return null;
        }

        ApiContext dbContext = context.HttpContext.RequestServices.GetRequiredService<ApiContext>();

        var dbPlayerInfo = await dbContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.AccountId == gameAccountId)
            .Select(x => new { x.ViewerId, x.UserData!.Name })
            .FirstOrDefaultAsync();

        logger.LogDebug(
            "PlayerInfo for game account {GameAccountId}: {@PlayerInfo}",
            gameAccountId,
            dbPlayerInfo
        );

        if (dbPlayerInfo is null)
        {
            return null;
        }

        PlayerInfo playerInfo =
            new()
            {
                AccountId = gameAccountId,
                Name = dbPlayerInfo.Name,
                ViewerId = dbPlayerInfo.ViewerId,
            };

        await cache.SetJsonAsync(
            cacheKey,
            playerInfo,
            new DistributedCacheEntryOptions()
            {
                // The ID token lasts for one hour. We may retain cached data past the expiry of the ID token, but
                // that should be okay, since the JWT authentication will return an unauthorized result before reaching
                // this code.
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            }
        );

        return playerInfo;
    }

    private class PlayerInfo
    {
        public required string AccountId { get; init; }

        public long ViewerId { get; init; }

        public required string Name { get; init; }
    }
}
