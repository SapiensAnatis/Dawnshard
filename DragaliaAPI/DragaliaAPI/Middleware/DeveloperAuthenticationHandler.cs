using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Middleware;

public class DeveloperAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DeveloperAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? tokenVar =
            Environment.GetEnvironmentVariable("DEVELOPER_TOKEN")
            ?? throw new NullReferenceException("No developer token specified!");

        string[] expectedTokens = tokenVar.Split("|");

        if (this.Request.Headers.Authorization.Count == 0)
        {
            this.Logger.LogDebug("No Authorization header found.");
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (
            !AuthenticationHeaderValue.TryParse(
                this.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authHeader
            )
        )
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Failed to parse Authorization header.")
            );
        }

        if (authHeader.Parameter is null || !expectedTokens.Contains(authHeader.Parameter))
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));

        this.Logger.LogDebug(
            "Authenticated using token {token}",
            authHeader.Parameter[..3] + "..."
        );

        Claim[] claims = { new(ClaimTypes.Role, "Developer"), };
        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
