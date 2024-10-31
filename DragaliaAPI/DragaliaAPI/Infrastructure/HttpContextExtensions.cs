using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using MessagePack;

namespace DragaliaAPI.Infrastructure;

internal static class HttpContextExtensions
{
    public static async Task WriteResultCodeResponse(this HttpContext context, ResultCode code)
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
