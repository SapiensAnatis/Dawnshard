using System.Net;

namespace DragaliaAPI.Infrastructure.Middleware;

public partial class NotFoundHandlerMiddleware
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

        Log.HTTP404On(this.logger, context.Request.Path);

        await context.WriteResultCodeResponse(NotFoundCode);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "HTTP 404 on {RequestPath}")]
        public static partial void HTTP404On(ILogger logger, PathString requestPath);
    }
}
