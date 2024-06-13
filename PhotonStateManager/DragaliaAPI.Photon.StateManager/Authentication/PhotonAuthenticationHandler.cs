using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Photon.StateManager.Authentication;

public class PhotonAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> authOptions,
    IOptionsMonitor<PhotonOptions> photonOptions,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(authOptions, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (
            !AuthenticationHeaderValue.TryParse(
                this.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authenticationHeader
            )
        )
        {
            Logger.LogDebug("Failed to parse Authorization header.");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authenticationHeader.Parameter is null)
        {
            this.Logger.LogDebug("AuthenticationHeader.Parameter was null");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authenticationHeader.Parameter != photonOptions.CurrentValue.Token)
        {
            this.Logger.LogInformation(
                "AuthenticationHeader.Parameter value {param} did not match configured token.",
                authenticationHeader.Parameter
            );
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }

        ClaimsIdentity identity = new(this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
