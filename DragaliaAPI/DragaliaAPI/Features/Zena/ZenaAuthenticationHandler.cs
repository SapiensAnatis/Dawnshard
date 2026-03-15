using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Zena;

public partial class ZenaAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ZenaAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? expectedToken = Environment.GetEnvironmentVariable("ZENA_TOKEN");

        if (expectedToken is null)
        {
            Log.FailedToAcquireZenaBearerToken(this.Logger);
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (
            !AuthenticationHeaderValue.TryParse(
                Request.Headers.Authorization,
                out AuthenticationHeaderValue? parsedValue
            )
        )
        {
            Log.FailedToParseAuthorizationHeader(this.Logger);
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (parsedValue is not { Scheme: "Bearer", Parameter: { } providedToken })
        {
            Log.AuthorizationHeaderDidNotMeetRequirements(this.Logger, parsedValue);
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (providedToken != expectedToken)
        {
            return Task.FromResult(
                AuthenticateResult.Fail($"Supplied token {providedToken} was not valid.")
            );
        }

        Claim[] claims = [new Claim(ClaimTypes.Role, "Zena")];
        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Failed to acquire Zena bearer token")]
        public static partial void FailedToAcquireZenaBearerToken(ILogger logger);
        [LoggerMessage(LogLevel.Information, "Failed to parse Authorization header.")]
        public static partial void FailedToParseAuthorizationHeader(ILogger logger);
        [LoggerMessage(LogLevel.Information, "Authorization header {@Header} did not meet requirements")]
        public static partial void AuthorizationHeaderDidNotMeetRequirements(ILogger logger, AuthenticationHeaderValue header);
    }
}
