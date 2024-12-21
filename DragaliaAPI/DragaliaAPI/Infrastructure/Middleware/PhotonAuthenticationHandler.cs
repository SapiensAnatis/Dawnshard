using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Database;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Infrastructure.Middleware;

public class PhotonAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> authOptions,
    IOptionsMonitor<PhotonOptions> photonOptions,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ApiContext apiContext
) : AuthenticationHandler<AuthenticationSchemeOptions>(authOptions, logger, encoder)
{
    public const string Role = "Photon";

    private readonly ApiContext apiContext = apiContext;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (
            !AuthenticationHeaderValue.TryParse(
                this.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authenticationHeader
            )
        )
        {
            this.Logger.LogDebug("Failed to parse Authorization header.");
            return AuthenticateResult.NoResult();
        }

        if (authenticationHeader.Parameter is null)
        {
            this.Logger.LogDebug("AuthenticationHeader.Parameter was null");
            return AuthenticateResult.NoResult();
        }

        if (authenticationHeader.Parameter != photonOptions.CurrentValue.Token)
        {
            this.Logger.LogInformation(
                "AuthenticationHeader.Parameter value {param} did not match configured token.",
                authenticationHeader.Parameter
            );
            return AuthenticateResult.Fail("Invalid token.");
        }

        if (
            !this.Request.Headers.TryGetValue("Auth-ViewerId", out StringValues viewerIdStr)
            || !long.TryParse(viewerIdStr, out long viewerId)
        )
        {
            return AuthenticateResult.Fail("Missing or malformed Auth-ViewerId header.");
        }

        string? accountId = await this
            .apiContext.Players.Where(x => x.ViewerId == viewerId)
            .Select(x => x.AccountId)
            .FirstOrDefaultAsync();

        if (accountId is null)
        {
            return AuthenticateResult.Fail($"No user found for viewer ID {viewerId}");
        }

        ClaimsIdentity identity = new(this.Scheme.Name);
        identity.AddClaim(new Claim(CustomClaimType.ViewerId, viewerId.ToString()));
        identity.AddClaim(new Claim(CustomClaimType.AccountId, accountId));
        identity.AddClaim(new Claim(ClaimTypes.Role, Role));

        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
