using Microsoft.AspNetCore.Mvc.Filters;

namespace DragaliaAPI.Infrastructure.Middleware;

internal partial class ResultCodeLoggingMiddleware(ILogger<ResultCodeLoggingMiddleware> logger)
    : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ResultCode? resultCode = null;

        if (
            context.Items.TryGetValue(nameof(ResultCode), out object? resultCodeObj)
            && resultCodeObj is ResultCode resultCodeValue
        )
        {
            resultCode = resultCodeValue;
        }

        LogLevel logLevel = resultCode is null or ResultCode.Success
            ? LogLevel.Information
            : LogLevel.Error;

        Log.EndpointResponded(logger, logLevel, context.Request.Path.ToString(), resultCode);

        return next(context);
    }

    private static partial class Log
    {
        [LoggerMessage("Endpoint {RequestPath} responded with result code {ResultCode}")]
        public static partial void EndpointResponded(
            ILogger logger,
            LogLevel level,
            string endpoint,
            ResultCode? resultCode
        );
    }
}
