using System.Net;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using MessagePack;

namespace DragaliaAPI.Middleware;

public class NotFoundHandlerMiddleware
{
    private readonly ILogger<NotFoundHandlerMiddleware> logger;
    private readonly RequestDelegate next;

    private const ResultCode NotFoundCode = ResultCode.CommonTimeout; // Shows error 'Failed to connect to the server'

    public NotFoundHandlerMiddleware(RequestDelegate next, ILoggerFactory logger)
    {
        this.next = next;
        this.logger = logger.CreateLogger<NotFoundHandlerMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode != (int)HttpStatusCode.NotFound)
            return;

        if (context.GetEndpoint() is not null)
            // Exclude controllers where we return this.NotFound() explicitly
            return;

        this.logger.LogInformation("HTTP 404 on {RequestPath}", context.Request.Path);
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        DragaliaResponse<ResultCodeResponse> gameResponse =
            new(new DataHeaders(NotFoundCode), new(NotFoundCode));

        await context.Response.Body.WriteAsync(
            MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
        );
    }
}
