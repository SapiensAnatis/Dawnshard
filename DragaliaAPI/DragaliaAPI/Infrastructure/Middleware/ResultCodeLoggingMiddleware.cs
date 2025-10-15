using System.Collections.Frozen;

namespace DragaliaAPI.Infrastructure.Middleware;

internal partial class ResultCodeLoggingMiddleware(ILogger<ResultCodeLoggingMiddleware> logger)
    : IMiddleware
{
    private static FrozenSet<ResultCode> NonErrorResultCodes { get; } =
    [
        ResultCode.Success,
        ResultCode.CommonMaintenance,
        ResultCode.CommonChangeDate,
        ResultCode.CommonTimeout,
        ResultCode.MatchingRoomIdNotFound,
        ResultCode.FriendIdsearchError,
        ResultCode.FriendTargetNone,
        ResultCode.FriendTargetAlready,
        ResultCode.FriendApplyExists,
        ResultCode.FriendCountLimit,
        ResultCode.FriendCountOtherLimit,
        ResultCode.FriendApplyCountLimit,
        ResultCode.FriendApplyCountOtherLimit,
    ];

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
        (code is null || NonErrorResultCodes.Contains(code.Value))
            ? LogLevel.Information
            : LogLevel.Error;

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
