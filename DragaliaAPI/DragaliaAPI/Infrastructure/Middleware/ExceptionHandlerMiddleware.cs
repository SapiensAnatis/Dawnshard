using Microsoft.AspNetCore.Diagnostics;

namespace DragaliaAPI.Infrastructure.Middleware;

public static class ExceptionHandlerMiddleware
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
            logger.LogWarning(exception, "Client cancelled request.");
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
}
