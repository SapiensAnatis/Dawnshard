using DragaliaAPI.Controllers;
using DragaliaAPI.MessagePack;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using MessagePack;

namespace DragaliaAPI.Middleware;

public class ExceptionHandlerMiddleware
{
    private const ResultCode ServerErrorCode = ResultCode.COMMON_SERVER_ERROR;

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

            this.logger.LogError("Encountered unhandled exception: {exception}", ex);

            context.Response.ContentType = CustomMessagePackOutputFormatter.ContentType;
            context.Response.StatusCode = 200;

            ResultCode code = ex is DragaliaException dragaliaException
                ? dragaliaException.Code
                : ServerErrorCode;

            DragaliaResponse<DataHeaders> gameResponse = new(new DataHeaders(code), code);

            await context.Response.Body.WriteAsync(
                MessagePackSerializer.Serialize(gameResponse, CustomResolver.Options)
            );
        }
    }
}
