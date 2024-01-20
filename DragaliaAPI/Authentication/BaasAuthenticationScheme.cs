using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Authentication;

public class BaasAuthenticationScheme : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IOptionsMonitor<BaasOptions> baasOptions;
    private readonly ApiContext apiContext;
    private readonly IBaasApi baasApi;

    public BaasAuthenticationScheme(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptionsMonitor<BaasOptions> baasOptions,
        ApiContext apiContext,
        IBaasApi baasApi,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder)
    {
        this.baasOptions = baasOptions;
        this.apiContext = apiContext;
        this.baasApi = baasApi;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (
            !AuthenticationHeaderValue.TryParse(
                this.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authenticationHeader
            )
        )
        {
            return AuthenticateResult.NoResult();
        }

        if (authenticationHeader is not { Scheme: "Bearer", Parameter: { } idToken })
            return AuthenticateResult.NoResult();

        JwtSecurityTokenHandler handler = new();

        TokenValidationResult tokenValidationResult = await handler.ValidateTokenAsync(
            idToken,
            new TokenValidationParameters()
            {
                IssuerSigningKeys = await this.baasApi.GetKeys(),
                ValidAudience = this.baasOptions.CurrentValue.TokenAudience,
                ValidIssuer = this.baasOptions.CurrentValue.TokenIssuer,
            }
        );

        if (!tokenValidationResult.IsValid)
            return AuthenticateResult.Fail(tokenValidationResult.Exception);

        JwtSecurityToken jwToken = (JwtSecurityToken)tokenValidationResult.SecurityToken;

        var playerInfo = this.apiContext.Players.Where(x => x.AccountId == jwToken.Subject)
            .Select(x => new { x.ViewerId, x.UserData!.Name })
            .FirstOrDefault();

        if (playerInfo is null)
            return AuthenticateResult.Fail($"No player found for subject ID {jwToken.Subject}");

        ClaimsIdentity identity = new("OAuth");

        identity.AddClaim(new Claim(CustomClaimType.AccountId, jwToken.Subject));
        identity.AddClaim(new Claim(CustomClaimType.PlayerName, playerInfo.Name));
        identity.AddClaim(new Claim(CustomClaimType.ViewerId, playerInfo.ViewerId.ToString()));

        AuthenticationTicket ticket = new(new ClaimsPrincipal(identity), this.Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
