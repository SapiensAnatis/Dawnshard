using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DragaliaAPI.Shared.PlayerDetails;

public class PlayerDetailsService : IPlayerDetailsService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public PlayerDetailsService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    private HttpContext Context => this.httpContextAccessor.HttpContext;

    private string? accountId;

    public string AccountId
    {
        get
        {
            this.accountId ??= this.Context.User.FindFirstValue(CustomClaimType.AccountId);

            return this.accountId
                ?? throw new InvalidOperationException("No AccountId claim value found");
            ;
        }
    }

    private long? viewerId;

    public long ViewerId
    {
        get
        {
            this.viewerId ??= long.Parse(
                this.Context.User.FindFirstValue(CustomClaimType.ViewerId)
                    ?? throw new InvalidOperationException("No ViewerId claim value found")
            );

            return this.viewerId
                ?? throw new InvalidOperationException("No ViewerId claim value found");
        }
    }
}
