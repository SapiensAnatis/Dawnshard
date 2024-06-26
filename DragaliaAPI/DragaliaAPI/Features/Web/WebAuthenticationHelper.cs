using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Features.Web;

public static class WebAuthenticationHelper
{
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        if (context.Request.Cookies.TryGetValue("idToken", out string? idToken))
        {
            context.Token = idToken;
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

        string accountId = jsonWebToken.Subject;

        ApiContext dbContext = context.HttpContext.RequestServices.GetRequiredService<ApiContext>();

        // TODO: Cache this query in Redis
        var playerInfo = await dbContext
            .Players.IgnoreQueryFilters()
            .Where(x => x.AccountId == accountId)
            .Select(x => new { x.ViewerId, x.UserData!.Name, })
            .FirstOrDefaultAsync();

        if (playerInfo is not null)
        {
            ClaimsIdentity playerIdentity =
                new(
                    [
                        new Claim(CustomClaimType.AccountId, accountId),
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
}
