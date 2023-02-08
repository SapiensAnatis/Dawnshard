using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using DragaliaAPI.Controllers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using MessagePack;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Middleware;

public class ExceptionHandlerMiddleware
{
    private const ResultCode ServerErrorCode = ResultCode.CommonServerError;

    private const string RefreshIdToken = "Is-Required-Refresh-Id-Token";
    private const string True = "true";

    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory logger)
    {
        this.next = next;
        this.logger = logger.CreateLogger<ExceptionHandlerMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            Endpoint? endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<SerializeExceptionAttribute>() == null)
                throw;

            if (context.RequestAborted.IsCancellationRequested)
            {
                this.logger.LogWarning(ex, "Client cancelled request.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return;
            }
            else if (ex is SessionException)
            {
                // Will send back to login
                this.logger.LogInformation(
                    "Returning session refresh request due to SessionException"
                );
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Headers.Add(RefreshIdToken, True);

                return;
            }

            context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
            context.Response.StatusCode = 200;

            ResultCode code = ex switch
            {
                DragaliaException d => d.Code,
                SecurityTokenExpiredException => ResultCode.IdTokenError,
                NotImplementedException => ResultCode.CommonTimeout,
                _ => ResultCode.CommonServerError
            };

            this.logger.LogError(
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
}
