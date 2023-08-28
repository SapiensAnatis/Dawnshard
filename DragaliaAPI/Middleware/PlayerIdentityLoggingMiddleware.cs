using DragaliaAPI.Shared.PlayerDetails;
using Serilog.Context;

namespace DragaliaAPI.Middleware;

public class PlayerIdentityLoggingMiddleware(IPlayerIdentityService playerIdentityService)
    : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            LogContext.PushProperty("AccountId", playerIdentityService.AccountId);
            LogContext.PushProperty("ViewerId", playerIdentityService.ViewerId);
        }

        return next.Invoke(context);
    }
}
