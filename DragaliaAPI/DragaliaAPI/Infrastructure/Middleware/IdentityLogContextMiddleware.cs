using DragaliaAPI.Shared.PlayerDetails;
using Serilog.Context;

namespace DragaliaAPI.Infrastructure.Middleware;

public class IdentityLogContextMiddleware(IPlayerIdentityService playerIdentityService)
    : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? accountId = null;
        long? viewerId = null;

        if (context.User.Identity is { IsAuthenticated: true })
        {
            if (context.User.HasClaim(x => x.Type == CustomClaimType.AccountId))
            {
                viewerId = playerIdentityService.ViewerId;
            }

            if (context.User.HasClaim(x => x.Type == CustomClaimType.ViewerId))
            {
                accountId = playerIdentityService.AccountId;
            }
        }

        using (LogContext.PushProperty("AccountId", accountId))
        using (LogContext.PushProperty("ViewerId", viewerId))
        {
            return next.Invoke(context);
        }
    }
}
