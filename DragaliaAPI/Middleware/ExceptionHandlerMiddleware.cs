using System.Net;
using DragaliaAPI.Controllers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using MessagePack;

namespace DragaliaAPI.Middleware;

public class ExceptionHandlerMiddleware
{
    private const ResultCode ServerErrorCode = ResultCode.CommonServerError;

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
        catch (SessionException)
        {
            this.logger.LogInformation(
                "Returning ID token refresh request due to SessionException"
            );
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Headers.Add("Is-Required-Refresh-Id-Token", "true");
        }
        catch (Exception ex)
        {
            Endpoint? endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<SerializeExceptionAttribute>() == null)
                throw;

            this.logger.LogError("Encountered unhandled exception: {exception}", ex);

            context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
            context.Response.StatusCode = 200;

            ResultCode code = ex is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ServerErrorCode;

            this.logger.LogError("Returning result_code {code}", code);

            DragaliaResponse<DataHeaders> gameResponse = new(new DataHeaders(code), code);

            await context.Response.Body.WriteAsync(
                MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
            );
        }
    }
}
