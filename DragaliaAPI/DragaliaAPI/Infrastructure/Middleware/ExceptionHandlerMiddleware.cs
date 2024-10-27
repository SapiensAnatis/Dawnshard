using DragaliaAPI.Controllers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using MessagePack;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Infrastructure.Middleware;

public static class ExceptionHandlerMiddleware
{
    private static readonly DistributedCacheEntryOptions CacheOptions =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public static async Task HandleAsync(HttpContext context)
    {
        string? deviceId = null;
        if (context.Request.Headers.TryGetValue("DeviceId", out StringValues values))
        {
            deviceId = values.FirstOrDefault();
        }

        IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        Exception? exception = exceptionHandlerPathFeature?.Error;

        ILogger logger = context
            .RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(ExceptionHandlerMiddleware));

        IDistributedCache cache = context.RequestServices.GetRequiredService<IDistributedCache>();

        if (exception is SecurityTokenExpiredException ex && deviceId is not null)
        {
            logger.LogDebug("ID token was expired. Expiry: {expiry}", ex.Expires);

            // Setting the below header should cause the client to route back to Nintendo login.
            // However, sometimes the client returns with the same invalid ID token.

            string redisKey = $"refresh_sent:{deviceId}";

            if (await cache.GetStringAsync(redisKey) is not null)
            {
                logger.LogError("Detected repeated SecurityTokenExpiredException.");
                await WriteResultCode(context, ResultCode.CommonAuthError);

                return;
            }

            await cache.SetStringAsync(redisKey, "true", CacheOptions);

            logger.LogDebug("Issuing ID token refresh request.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.Headers.Append("Is-Required-Refresh-Id-Token", "true");
        }
        else if (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Client cancelled request.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else
        {
            ResultCode code = exception is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ResultCode.CommonServerError;

            logger.LogError(
                exception,
                "Encountered unhandled exception. Returning result_code {code}",
                code
            );

            await WriteResultCode(context, code);
        }
    }

    private static async Task WriteResultCode(HttpContext context, ResultCode code)
    {
        context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Items[nameof(ResultCode)] = code;

        DragaliaResponse<DataHeaders> gameResponse = new(new DataHeaders(code), code);

        await context.Response.Body.WriteAsync(
            MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
        );
    }
}
