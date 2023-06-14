using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DragaliaAPI.Shared.PlayerDetails;

public class PlayerIdentityService : IPlayerIdentityService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public PlayerIdentityService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    private HttpContext Context => this.httpContextAccessor.HttpContext;

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

    public long ViewerId
    {
        get
        {
            if (impersonationContext != null)
            {
                return impersonationContext.ViewerId
                    ?? throw new InvalidOperationException(
                        "No ViewerId in current impersonation session"
                    );
            }

            this.viewerId ??= long.Parse(
                this.Context.User.FindFirstValue(CustomClaimType.ViewerId)
                    ?? throw new InvalidOperationException("No ViewerId claim value found")
            );

            return this.viewerId
                ?? throw new InvalidOperationException("No ViewerId claim value found");
        }
    }

    private ImpersonationContext? impersonationContext;

    public IDisposable StartUserImpersonation(string? account = null, long? viewer = null)
    {
        if (impersonationContext != null)
        {
            throw new InvalidOperationException(
                "Can't start impersonation context when a user is already being impersonated."
            );
        }

        impersonationContext = new ImpersonationContext(this, account, viewer);
        return impersonationContext;
    }

    internal void StopUserImpersonation()
    {
        this.impersonationContext = null;
    }
}

internal class ImpersonationContext : IDisposable
{
    private readonly PlayerIdentityService playerIdentityService;
    public readonly string? AccountId;
    public readonly long? ViewerId;

    public ImpersonationContext(
        PlayerIdentityService playerIdentityService,
        string? accountId,
        long? viewerId
    )
    {
        this.playerIdentityService = playerIdentityService;
        this.AccountId = accountId;
        this.ViewerId = viewerId;
    }

    public void Dispose()
    {
        playerIdentityService.StopUserImpersonation();
    }
}
