using DragaliaAPI.Helpers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Middleware;

public class DailyResetMiddleware
{
    private readonly RequestDelegate next;
    private readonly IServiceProvider serviceProvider;

    public DailyResetMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        this.next = next;
        this.serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (
            context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is null
            && context.GetEndpoint()?.Metadata.GetMetadata<BypassDailyResetAttribute>() is null
            && context.Request.Headers.TryGetValue("SID", out StringValues value)
        )
        {
            string? sessionId = value.FirstOrDefault();
            if (sessionId != null)
            {
                using IServiceScope scope = serviceProvider.CreateScope();

                ISessionService sessionService =
                    scope.ServiceProvider.GetRequiredService<ISessionService>();
                IResetHelper resetHelper = scope.ServiceProvider.GetRequiredService<IResetHelper>();

                Session session = await sessionService.LoadSessionSessionId(sessionId);
                if (resetHelper.LastDailyReset > session.LoginTime)
                {
                    context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
                    context.Response.StatusCode = 200;

                    DragaliaResponse<DataHeaders> gameResponse =
                        new(
                            new DataHeaders(ResultCode.CommonChangeDate),
                            ResultCode.CommonChangeDate
                        );

                    await context.Response.Body.WriteAsync(
                        MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
                    );

                    return;
                }
            }
        }

        await this.next(context);
    }
}
