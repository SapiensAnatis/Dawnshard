using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Infrastructure.Middleware;

public class LogContextMiddleware(IPlayerIdentityService playerIdentityService) : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            if (context.User.HasClaim(x => x.Type == CustomClaimType.AccountId))
            {
                LogContext.PushProperty("AccountId", playerIdentityService.AccountId);
            }

            if (context.User.HasClaim(x => x.Type == CustomClaimType.ViewerId))
            {
                LogContext.PushProperty("ViewerId", playerIdentityService.ViewerId);
            }
        }

        if (
            context.Request.Headers.TryGetValue(Headers.RequestToken, out StringValues requestToken)
        )
        {
            LogContext.PushProperty("RequestToken", requestToken.ToString());
        }

        return next.Invoke(context);
    }
}
