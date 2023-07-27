using System.Net;
using DragaliaAPI.Controllers;
using DragaliaAPI.Extensions;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private static readonly DistributedCacheEntryOptions CacheOptions =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ILogger<ExceptionHandlerMiddleware> logger,
        IDistributedCache cache
    )
    {
        Endpoint? endpoint = context.GetEndpoint();
        bool serializeException =
            endpoint?.Metadata.GetMetadata<SerializeExceptionAttribute>() is not null;

        string? deviceId = null;
        if (context.Request.Headers.TryGetValue("DeviceId", out StringValues values))
            deviceId = values.FirstOrDefault();

        try
        {
            await this.next(context);
        }
        catch (SecurityTokenExpiredException ex) when (serializeException && deviceId is not null)
        {
            logger.LogDebug("ID token was expired. Expiry: {expiry}", ex.Expires);

            // This should cause the client to route back to Nintendo login.
            // However, sometimes the client returns with the same invalid ID token.

            string redisKey = $"refresh_sent:{deviceId}";

            if (await cache.GetStringAsync(redisKey) is not null)
            {
                logger.LogWarning("Detected repeated SecurityTokenExpiredException.");
                throw new DragaliaException(
                    ResultCode.CommonAuthError,
                    "Detected repeated SecurityTokenExpiredException."
                );
            }

            logger.LogDebug("Issuing ID token refresh request.");
            await cache.SetStringAsync(redisKey, "true", CacheOptions);

            context.Response.StatusCode = 400;
            context.Response.Headers.Add("Is-Required-Refresh-Id-Token", "true");
            return;
        }
        catch (Exception ex) when (serializeException)
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Client cancelled request.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return;
            }

            context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
            context.Response.StatusCode = 200;

            ResultCode code = ex is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ResultCode.CommonServerError;

            logger.LogError(
                ex,
                "Encountered unhandled exception. Returning result_code {code}",
                code
            );

            DragaliaResponse<DataHeaders> gameResponse = new(new DataHeaders(code), code);

            await context.Response.Body.WriteAsync(
                MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
            );
        }
    }

    private static string? GetRedisKey(string? deviceId)
    {
        return deviceId is null ? null : $"refresh_sent:{deviceId}";
    }
}
