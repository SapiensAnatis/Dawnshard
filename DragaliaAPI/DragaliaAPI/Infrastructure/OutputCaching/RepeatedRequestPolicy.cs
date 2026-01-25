using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Infrastructure.OutputCaching;

internal class RepeatedRequestPolicy(ILogger<RepeatedRequestPolicy> logger) : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        context.EnableOutputCaching = true;

        bool shouldCache = GetShouldCache(context);

        context.AllowCacheLookup = shouldCache;
        context.AllowCacheStorage = shouldCache;

        context.CacheVaryByRules.HeaderNames = new StringValues([
            Headers.RequestToken,
            Headers.SessionId,
        ]);

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        logger.LogInformation("Detected repeated Request-Token. Serving cached output.");

        // All cached responses are successes
        context.HttpContext.Items[nameof(ResultCode)] = ResultCode.Success;

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

        // This will always be true (and the request will always be a game request) because of how the middleware
        // branching is done in Program.cs.
        //
        // if (!RoutePrefixes.List.Any(x => request.PathBase == x))
        // {
        //     return false;
        // }

        // Request likely did not come from a game client. Could be from an integration test or Photon Server.
        if (!request.Headers.ContainsKey(Headers.RequestToken))
        {
            return false;
        }

        // We do not want to cache requests globally - make sure this request can be keyed to a particular user.
        if (!request.Headers.ContainsKey(Headers.SessionId))
        {
            return false;
        }

        return true;
    }
}
