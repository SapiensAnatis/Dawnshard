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

        LogLevel logLevel =
            resultCode is null || IsSuccessResultCode(resultCode.Value)
                ? LogLevel.Information
                : LogLevel.Error;

        Log.EndpointResponded(logger, logLevel, context.Request.Path.ToString(), resultCode);
    }

    private static bool IsSuccessResultCode(ResultCode code) =>
        code is ResultCode.Success or ResultCode.CommonChangeDate or ResultCode.CommonMaintenance;

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
