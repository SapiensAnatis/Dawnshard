using DragaliaAPI.Controllers;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using Microsoft.Extensions.Primitives;
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
            throw new DragaliaException(
                ResultCode.SESSION_SESSION_ID_NOT_FOUND,
                "Missing SID header!"
            );

        string? sessionIdString = sessionId;

        if (sessionIdString == null)
            throw new DragaliaException(
                ResultCode.SESSION_SESSION_ID_NOT_FOUND,
                "Null SID header!"
            );

        context.Items.Add(
            "DeviceAccountId",
            await sessionService.GetDeviceAccountId_SessionId(sessionIdString)
        );

        await this.next.Invoke(context);
    }
}
