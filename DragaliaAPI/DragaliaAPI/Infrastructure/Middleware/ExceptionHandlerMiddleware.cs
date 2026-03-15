using Microsoft.AspNetCore.Diagnostics;

namespace DragaliaAPI.Infrastructure.Middleware;

public static partial class ExceptionHandlerMiddleware
{
    public static async Task HandleAsync(HttpContext context)
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        Exception? exception = exceptionHandlerPathFeature?.Error;

        ILogger logger = context
            .RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(ExceptionHandlerMiddleware));

        if (context.RequestAborted.IsCancellationRequested)
        {
            Log.ClientCancelledRequest(logger, exception);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else
        {
            ResultCode code = exception is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ResultCode.CommonServerError;

            await context.WriteResultCodeResponse(code);
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Client cancelled request.")]
        public static partial void ClientCancelledRequest(ILogger logger, Exception exception);
    }
}
