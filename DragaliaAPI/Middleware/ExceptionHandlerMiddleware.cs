﻿using System.Diagnostics;
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
            /*else if (ex is SecurityTokenExpiredException)
            {
                // Will send back to BaaS to login
                this.logger.LogInformation(
                    "Returning ID token refresh request due to SecurityTokenExpiredException"
                );
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Headers.Add(RefreshIdToken, True);

                return;
            }*/

            this.logger.LogError(ex, "Encountered unhandled exception");

            context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
            context.Response.StatusCode = 200;

            ResultCode code = ex is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ServerErrorCode;

            code = ex is SecurityTokenExpiredException ? ResultCode.IdTokenError : code;

            this.logger.LogError("Returning result_code {code}", code);

            DragaliaResponse<DataHeaders> gameResponse = new(new DataHeaders(code), code);

            await context.Response.Body.WriteAsync(
                MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
            );
        }
    }
}
