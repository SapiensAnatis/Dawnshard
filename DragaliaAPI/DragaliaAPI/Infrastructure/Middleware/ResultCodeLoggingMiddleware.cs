using System.Collections.Frozen;

namespace DragaliaAPI.Infrastructure.Middleware;

internal partial class ResultCodeLoggingMiddleware(ILogger<ResultCodeLoggingMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        ResultCode? resultCode = null;

        if (
            context.Items.TryGetValue(nameof(ResultCode), out object? resultCodeObj)
            && resultCodeObj is ResultCode resultCodeValue
        )
        {
            resultCode = resultCodeValue;
        }

        LogLevel logLevel = GetLogLevelFromResultCode(resultCode);

        Log.EndpointResponded(logger, logLevel, context.Request.Path.ToString(), resultCode);
    }

    private static LogLevel GetLogLevelFromResultCode(ResultCode? code) =>
        code switch
        {
            null
            or ResultCode.Success
            or ResultCode.CommonChangeDate
            or ResultCode.CommonMaintenance
            or ResultCode.CommonTimeout
                => LogLevel.Information,
            _ => LogLevel.Error
        };

    private static partial class Log
    {
        [LoggerMessage("Endpoint {RequestPath} responded with result code {ResultCode}")]
        public static partial void EndpointResponded(
            ILogger logger,
            LogLevel level,
            string requestPath,
            ResultCode? resultCode
        );
    }
}
