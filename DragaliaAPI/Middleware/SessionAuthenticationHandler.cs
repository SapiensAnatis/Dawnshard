using DragaliaAPI.Controllers;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using Microsoft.Extensions.Primitives;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DragaliaAPI.Middleware;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ISessionService sessionService;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ISessionService sessionService
    ) : base(options, logger, encoder, clock)
    {
        this.sessionService = sessionService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (this.Context.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>() is null)
            return AuthenticateResult.NoResult();

        if (!this.Request.Headers.TryGetValue("SID", out StringValues value))
            return AuthenticateResult.Fail("Missing SID header");

        string? sid = value.FirstOrDefault();

        if (sid is null)
            return AuthenticateResult.Fail("Invalid SID header: value was null");

        // This will throw SessionException if not found, and return a BadRequest which prompts the client to re-login
        Session session = await this.sessionService.LoadSessionSessionId(sid);

        Claim[] claims = new[]
        {
            new Claim(CustomClaimType.AccountId, session.DeviceAccountId),
            new Claim(CustomClaimType.ViewerId, session.ViewerId.ToString())
        };
        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
