using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Controllers;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Middleware;

public class DeveloperAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DeveloperAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        IEnumerable<string> expectedTokens = this.GetDeveloperTokens().ToList();

        // TODO: REMOVE
        this.Logger.LogDebug("{@expectedTokens}", expectedTokens.AsEnumerable());

        if (!this.Request.Headers.Authorization.Any())
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

    private IEnumerable<string> GetDeveloperTokens()
    {
        string? first = Environment.GetEnvironmentVariable("DEVELOPER_TOKEN");
        if (first is not null)
            yield return first;

        for (int i = 2; i <= 9; i++)
        {
            string? additional = Environment.GetEnvironmentVariable($"DEVELOPER_TOKEN_{i}");
            if (additional is not null)
                yield return additional;
        }
    }
}
