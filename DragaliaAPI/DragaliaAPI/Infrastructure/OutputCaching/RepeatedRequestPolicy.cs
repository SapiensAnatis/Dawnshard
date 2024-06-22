using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Infrastructure.OutputCaching;

public class RepeatedRequestPolicy(ILogger<RepeatedRequestPolicy> logger) : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        context.EnableOutputCaching = true;

        bool shouldCache = GetShouldCache(context);

        context.AllowCacheLookup = shouldCache;
        context.AllowCacheStorage = shouldCache;

        context.CacheVaryByRules.HeaderNames = new StringValues(
            [Headers.RequestToken, Headers.SessionId]
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        context.HttpContext.Request.Headers.TryGetValue(
            Headers.RequestToken,
            out StringValues requestTokenValues
        );

        logger.LogInformation(
            "Serving cached output for request token {RequestToken}",
            requestTokenValues.FirstOrDefault()
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        if (context.HttpContext.Response.StatusCode != StatusCodes.Status200OK)
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        // Disallow caching of errors
        if (
            context.HttpContext.Items.TryGetValue(nameof(ResultCode), out object? resultCodeObj)
            && resultCodeObj is not ResultCode.Success
        )
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    private static bool GetShouldCache(OutputCacheContext context)
    {
        HttpRequest request = context.HttpContext.Request;

        if (!HttpMethods.IsPost(request.Method))
        {
            return false;
        }

        if (!RoutePrefixes.List.Any(x => request.Path.StartsWithSegments(x)))
        {
            return false;
        }

        return true;
    }
}
