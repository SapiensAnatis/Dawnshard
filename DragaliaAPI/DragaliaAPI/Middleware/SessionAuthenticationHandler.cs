using System.Security.Claims;
using System.Text.Encodings.Web;
using DragaliaAPI.Database;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Middleware;

public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ISessionService sessionService;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ApiContext apiContext;

    private const string SessionExpired = "Session-Expired";
    private const string True = "true";

    public const string LastLoginTime = "LastLoginTime";

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISessionService sessionService,
        IWebHostEnvironment webHostEnvironment,
        ApiContext apiContext
    )
        : base(options, logger, encoder)
    {
        this.sessionService = sessionService;
        this.webHostEnvironment = webHostEnvironment;
        this.apiContext = apiContext;
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

        string deviceAccountId;
        string viewerId;

        try
        {
            Session session = await this.sessionService.LoadSessionSessionId(sid);

            deviceAccountId = session.DeviceAccountId;
            viewerId = session.ViewerId.ToString();

            this.Context.Items[LastLoginTime] = session.LoginTime;
        }
        catch (SessionException)
        {
            return AuthenticateResult.Fail($"Failed to look up SID {sid}");
        }

        Session? impersonatedSession = await this.sessionService.LoadImpersonationSession(
            deviceAccountId
        );

        if (impersonatedSession != null)
        {
            this.Logger.LogInformation(
                "User impersonation activated: {@impersonation}",
                new { impersonatedSession.DeviceAccountId, impersonatedSession.ViewerId }
            );

            deviceAccountId = impersonatedSession.DeviceAccountId;
            viewerId = impersonatedSession.ViewerId.ToString();
        }

        List<Claim> claims =
            new()
            {
                new(CustomClaimType.AccountId, deviceAccountId),
                new(CustomClaimType.ViewerId, viewerId)
            };

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
        this.Response.Headers.Append(SessionExpired, True);

        return Task.CompletedTask;
    }
}
