using System.Collections.Frozen;

namespace DragaliaAPI.Infrastructure.Middleware;

internal partial class ResultCodeLoggingMiddleware(ILogger<ResultCodeLoggingMiddleware> logger)
    : IMiddleware
{
    private static readonly FrozenSet<ResultCode> NonErrorResultCodes = new[]
    {
        ResultCode.Success,
        ResultCode.CommonChangeDate,
        ResultCode.CommonMaintenance,
    }.ToFrozenSet();

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

        LogLevel logLevel =
            resultCode is null || NonErrorResultCodes.Contains(resultCode.Value)
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
            string requestPath,
            ResultCode? resultCode
        );
    }
}
