using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace DragaliaAPI.Shared.PlayerDetails;

public class PlayerIdentityService : IPlayerIdentityService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<PlayerIdentityService> logger;

    public PlayerIdentityService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<PlayerIdentityService> logger
    )
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    private HttpContext Context => this.httpContextAccessor.HttpContext!;

    private string? accountId;

    public string AccountId
    {
        get
        {
            if (impersonationContext != null)
            {
                return impersonationContext.AccountId
                    ?? throw new InvalidOperationException(
                        "No AccountId in current impersonation session"
                    );
            }

            this.accountId ??= this.Context.User.FindFirstValue(CustomClaimType.AccountId);

            return this.accountId
                ?? throw new InvalidOperationException("No AccountId claim value found");
        }
    }

    private long? viewerId;

    public long? ViewerId
    {
        get
        {
            if (impersonationContext != null)
            {
                return impersonationContext.ViewerId;
            }

            if (this.viewerId == null)
            {
                string? viewerIdString = this.Context.User.FindFirstValue(CustomClaimType.ViewerId);

                if (viewerIdString != null)
                {
                    this.viewerId = long.Parse(viewerIdString);
                }
            }

            return this.viewerId;
        }
    }

    private ImpersonationContext? impersonationContext;

    public IDisposable StartUserImpersonation(string account, long? viewer = null)
    {
        if (impersonationContext != null)
        {
            throw new InvalidOperationException(
                "Can't start impersonation context when a user is already being impersonated."
            );
        }

        impersonationContext = new ImpersonationContext(this, account, viewer);

        logger.LogDebug(
            "Starting user impersonation: {@context}",
            new { impersonationContext.AccountId, impersonationContext.ViewerId }
        );

        return impersonationContext;
    }

    internal void StopUserImpersonation()
    {
        logger.LogDebug(
            "Stopping user impersonation: {@context}",
            new { impersonationContext!.AccountId, impersonationContext.ViewerId }
        );

        this.impersonationContext = null;
    }
}

internal class ImpersonationContext : IDisposable
{
    private readonly PlayerIdentityService playerIdentityService;
    public readonly string? AccountId;
    public readonly long? ViewerId;

    private readonly IDisposable? accountIdPropertyScope;
    private readonly IDisposable? viewerIdPropertyScope;

    public ImpersonationContext(
        PlayerIdentityService playerIdentityService,
        string? accountId,
        long? viewerId
    )
    {
        this.playerIdentityService = playerIdentityService;
        this.AccountId = accountId;
        this.ViewerId = viewerId;

        this.accountIdPropertyScope = LogContext.PushProperty(
            "ImpersonatedAccountId",
            this.AccountId
        );
        this.viewerIdPropertyScope = LogContext.PushProperty("ImpersonatedViewerId", this.ViewerId);
    }

    public void Dispose()
    {
        this.viewerIdPropertyScope?.Dispose();
        this.accountIdPropertyScope?.Dispose();

        playerIdentityService.StopUserImpersonation();
    }
}
