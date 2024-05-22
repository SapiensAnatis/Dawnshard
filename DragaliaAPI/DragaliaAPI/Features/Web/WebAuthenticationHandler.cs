using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using DragaliaAPI.Database;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DragaliaAPI.Features.Web;

public static class WebAuthenticationHelper
{
    public const string SchemeName = "WebScheme";

    public const string PolicyName = "WebPolicy";

    public static async Task OnMessageReceived(MessageReceivedContext context)
    {
        if (context.Request.Cookies.TryGetValue("idToken", out string? idToken))
        {
            context.Token = idToken;
        }

        IBaasApi baasApi = context.HttpContext.RequestServices.GetRequiredService<IBaasApi>();
        BaasOptions options = context.Request.HttpContext.RequestServices.GetRequiredService<IOptions<BaasOptions>>().Value;


        // TODO: submit a PR to baas to add openid config endpoint, so that we can use Authority and not fetch keys manually
        // https://developer.okta.com/docs/reference/api/oidc/#well-known-openid-configuration
        context.Options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.TokenIssuer,
            ValidAudience = options.TokenAudience,
            IssuerSigningKeys = await baasApi.GetKeys()
        };

    }

    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.SecurityToken is not JsonWebToken jsonWebToken)
        {
            throw new UnreachableException("What the fuck?");
        }

        string accountId = jsonWebToken.Subject;

        ApiContext dbContext = context.HttpContext.RequestServices.GetRequiredService<ApiContext>();

        var playerInfo = await dbContext
            .Players
            .IgnoreQueryFilters()
            .Where(x => x.AccountId == accountId)
            .Select(x => new
            {
                x.ViewerId,
                x.UserData!.LastSaveImportTime,
                x.UserData.Name,
            })
            .FirstOrDefaultAsync();

        if (playerInfo is null)
        {
            context.Fail("Unknown player");
            return;
        }

        return;
    }
}
