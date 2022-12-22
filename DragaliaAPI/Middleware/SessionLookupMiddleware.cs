using DragaliaAPI.Controllers;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Abstractions;
using DragaliaAPI.Services.Exceptions;

namespace DragaliaAPI.Middleware;

public class SessionLookupMiddleware
{
    private readonly RequestDelegate next;

    public SessionLookupMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, ISessionService sessionService)
    {
        Endpoint? controller = context.GetEndpoint();

        if (
            controller is null
            || controller.Metadata.GetMetadata<DragaliaControllerAttribute>() is null
            || controller.Metadata.GetMetadata<NoSessionAttribute>() is not null
        )
        {
            await this.next.Invoke(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("SID", out StringValues sessionId))
            throw new BadHttpRequestException("Missing SID header!");

        string? sessionIdString = sessionId;

        try
        {
            context.Items.Add(
                "DeviceAccountId",
                await sessionService.GetDeviceAccountId_SessionId(
                    sessionIdString ?? throw new BadHttpRequestException("Null SID header!")
                )
            );
        }
        catch (SessionException)
        {
            IActionResultExecutor<ObjectResult> executor =
                context.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();

            ActionContext actionContext =
                new(context, context.GetRouteData(), new ActionDescriptor());

            actionContext.HttpContext.Response.ContentType = "application/octet-stream";

            await executor.ExecuteAsync(
                actionContext,
                new OkObjectResult(
                    new DragaliaResponse<ResultCodeData>(
                        new(ResultCode.COMMON_SESSION_RESTORE_ERROR),
                        ResultCode.COMMON_SESSION_RESTORE_ERROR
                    )
                )
            );

            return;
        }

        await this.next.Invoke(context);
    }
}
