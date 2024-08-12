using Microsoft.Extensions.Primitives;
using Serilog.Context;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Infrastructure.Middleware;

/// <summary>
/// Middleware to add headers to the log context. Unlike <see cref="IdentityLogContextMiddleware"/>, this runs
/// prior to authentication so can diagnose issues that occur before authentication in the pipeline.
/// </summary>
public class HeaderLogContextMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? deviceId = GetHeader(context, Headers.DeviceId);
        string? requestToken = GetHeader(context, Headers.RequestToken);

        using (LogContext.PushProperty("DeviceId", deviceId))
        using (LogContext.PushProperty("RequestToken", requestToken))
        {
            return next(context);
        }
    }

    private static string? GetHeader(HttpContext context, string name) =>
        context.Request.Headers.TryGetValue(name, out StringValues headerValues)
            ? headerValues.FirstOrDefault()
            : null;
}
