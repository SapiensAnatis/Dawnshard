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
    private readonly IConfiguration configuration;

    public DeveloperAuthenticationHandler(
        IConfiguration configuration,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        this.configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (this.Context.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>() is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string expectedToken =
            Environment.GetEnvironmentVariable("DEVELOPER_TOKEN")
            ?? throw new NullReferenceException("No developer token specified!");

        if (!this.Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));

        try
        {
            AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(
                this.Request.Headers["Authorization"]
            );

            if (authHeader.Parameter != expectedToken)
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header!"));
        }

        Claim[] claims = new[] { new Claim(ClaimTypes.NameIdentifier, "Developer"), };
        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
