using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Photon.StateManager.Authentication;

public partial class PhotonAuthenticationHandler(
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
            Log.FailedToParseAuthorizationHeader(Logger);
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authenticationHeader.Parameter is null)
        {
            Log.AuthenticationHeaderParameterWasNull(this.Logger);
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authenticationHeader.Parameter != photonOptions.CurrentValue.Token)
        {
            Log.AuthenticationHeaderParameterValueDidNotMatchConfiguredToken(
                this.Logger,
                authenticationHeader.Parameter
            );
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }

        ClaimsIdentity identity = new(this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Failed to parse Authorization header.")]
        public static partial void FailedToParseAuthorizationHeader(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "AuthenticationHeader.Parameter was null")]
        public static partial void AuthenticationHeaderParameterWasNull(ILogger logger);

        [LoggerMessage(
            LogLevel.Information,
            "AuthenticationHeader.Parameter value {param} did not match configured token."
        )]
        public static partial void AuthenticationHeaderParameterValueDidNotMatchConfiguredToken(
            ILogger logger,
            string param
        );
    }
}
