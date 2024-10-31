using System.Net;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using MessagePack;

namespace DragaliaAPI.Infrastructure.Middleware;

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
        await this.next(context);

        if (context.Response.StatusCode != (int)HttpStatusCode.NotFound)
        {
            return;
        }

        // Exclude controllers where we return this.NotFound() explicitly
        if (context.GetEndpoint() is not null)
        {
            return;
        }

        this.logger.LogInformation("HTTP 404 on {RequestPath}", context.Request.Path);

        await context.WriteResultCodeResponse(NotFoundCode);
    }
}
