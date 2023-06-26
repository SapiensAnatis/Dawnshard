using DragaliaAPI.Helpers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace DragaliaAPI.Middleware;

public class DailyResetMiddleware
{
    private readonly RequestDelegate next;

    public DailyResetMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IResetHelper resetHelper)
    {
        if (
            context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is null
            && context.GetEndpoint()?.Metadata.GetMetadata<BypassDailyResetAttribute>() is null
            && context.Items.TryGetValue(
                SessionAuthenticationHandler.LastLoginTime,
                out object? lastLoginTimeObj
            )
            && lastLoginTimeObj is DateTimeOffset lastLoginTime
        )
        {
            if (resetHelper.LastDailyReset > lastLoginTime)
            {
                context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
                context.Response.StatusCode = 200;

                DragaliaResponse<DataHeaders> gameResponse =
                    new(new DataHeaders(ResultCode.CommonChangeDate), ResultCode.CommonChangeDate);

                await context.Response.Body.WriteAsync(
                    MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
                );

                return;
            }
        }

        await this.next(context);
    }
}
