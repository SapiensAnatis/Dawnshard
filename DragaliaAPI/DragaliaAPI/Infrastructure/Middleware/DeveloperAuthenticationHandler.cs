using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Middleware;

public class DeveloperAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IWebHostEnvironment environment;

    public DeveloperAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IWebHostEnvironment environment
    )
        : base(options, logger, encoder)
    {
        this.environment = environment;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (this.environment.IsDevelopment())
        {
            return Task.FromResult(GetSuccessResult());
        }

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

        return Task.FromResult(GetSuccessResult());
    }

    private AuthenticateResult GetSuccessResult()
    {
        Claim[] claims = { new(ClaimTypes.Role, "Developer"), };
        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
