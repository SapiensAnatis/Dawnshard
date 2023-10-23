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
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Middleware;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ISessionService sessionService;

    private const string SessionExpired = "Session-Expired";
    private const string True = "true";

    public const string LastLoginTime = "LastLoginTime";

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ISessionService sessionService
    )
        : base(options, logger, encoder, clock)
    {
        this.sessionService = sessionService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!this.Request.Headers.TryGetValue("SID", out StringValues value))
        {
            this.Logger.LogDebug("SID header was missing.");
            return AuthenticateResult.NoResult();
        }

        string? sid = value.FirstOrDefault();

        if (sid is null)
            return AuthenticateResult.Fail("Invalid SID header: value was null");

        List<Claim> claims = new();

        try
        {
            Session session = await this.sessionService.LoadSessionSessionId(sid);

            claims.Add(new(CustomClaimType.AccountId, session.DeviceAccountId));
            claims.Add(new(CustomClaimType.ViewerId, session.ViewerId.ToString()));

            this.Context.Items[LastLoginTime] = session.LoginTime;
        }
        catch (SessionException)
        {
            return AuthenticateResult.Fail($"Failed to look up SID {sid}");
        }

        ClaimsIdentity identity = new(claims, this.Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // Should make the client go back to /tool/reauth
        this.Logger.LogDebug("Returning Session-Expired BadRequest response");
        this.Response.StatusCode = 400;
        this.Response.Headers.Add(SessionExpired, True);

        return Task.CompletedTask;
    }
}
