using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Middleware;

public class PhotonAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Role = "Photon";

    private readonly IUserDataRepository userDataRepository;

    public PhotonAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserDataRepository userDataRepository
    )
        : base(options, logger, encoder, clock)
    {
        this.userDataRepository = userDataRepository;
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
            Logger.LogDebug("Failed to parse Authorization header.");
            return AuthenticateResult.NoResult();
        }

        if (authenticationHeader.Parameter is null)
        {
            Logger.LogDebug("AuthenticationHeader.Parameter was null");
            return AuthenticateResult.NoResult();
        }

        string configuredToken =
            Environment.GetEnvironmentVariable("PHOTON_TOKEN")
            ?? throw new ArgumentNullException("PHOTON_TOKEN environment variable not set!");

        if (authenticationHeader.Parameter != configuredToken)
        {
            Logger.LogInformation(
                "AuthenticationHeader.Parameter value {param} did not match configured token.",
                authenticationHeader.Parameter
            );
            return AuthenticateResult.Fail("Invalid token.");
        }

        if (
            !Request.Headers.TryGetValue("Auth-ViewerId", out StringValues viewerIdStr)
            || !long.TryParse(viewerIdStr, out long viewerId)
        )
        {
            return AuthenticateResult.Fail("Missing or malformed Auth-ViewerId header.");
        }

        string? accountId = (
            await this.userDataRepository.GetViewerData(viewerId).SingleOrDefaultAsync()
        )?.DeviceAccountId;

        if (accountId is null)
        {
            return AuthenticateResult.Fail($"No user found for viewer ID {viewerId}");
        }

        ClaimsIdentity identity = new(Scheme.Name);
        identity.AddClaim(new Claim(CustomClaimType.ViewerId, viewerId.ToString()));
        identity.AddClaim(new Claim(CustomClaimType.AccountId, accountId));
        identity.AddClaim(new Claim(ClaimTypes.Role, Role));

        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
