using System.Security.Claims;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace DragaliaAPI.Middleware;

public class AccountIdEnricherMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public AccountIdEnricherMiddleware(RequestDelegate next, ILoggerFactory logger)
    {
        this.next = next;
        this.logger = logger.CreateLogger<ExceptionHandlerMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? accountId = context.User.FindFirstValue(CustomClaimType.AccountId);
        using (LogContext.PushProperty(CustomClaimType.AccountId, accountId ?? "anonymous"))
        {
            await this.next(context);
        }
    }
}
