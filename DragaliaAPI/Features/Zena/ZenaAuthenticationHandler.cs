using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Zena;

public class ZenaAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
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
            this.Logger.LogWarning("Failed to acquire Zena bearer token");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (
            !AuthenticationHeaderValue.TryParse(
                Request.Headers.Authorization,
                out AuthenticationHeaderValue? parsedValue
            )
        )
        {
            this.Logger.LogInformation("Failed to parse Authorization header.");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (parsedValue is not { Scheme: "Bearer", Parameter: { } providedToken })
        {
            this.Logger.LogInformation(
                "Authorization header {@Header} did not meet requirements",
                parsedValue
            );
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
}
