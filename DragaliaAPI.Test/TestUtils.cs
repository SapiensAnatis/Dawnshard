using System.Net.Http.Headers;
using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using FluentAssertions.Equivalency;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Xunit.Abstractions;

namespace DragaliaAPI.Test;

public static class TestUtils
{
    public static HttpContent CreateMsgpackContent(object content, string sessionId = "session_id")
    {
        ByteArrayContent result = new(MessagePackSerializer.Serialize(content));
        result.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        result.Headers.Add("SID", sessionId);
        return result;
    }

    public static string MsgpackBytesToPrettyJson(byte[] content)
    {
        string json = MessagePackSerializer.ConvertToJson(
            content,
            ContractlessStandardResolver.Options
        );
        using var jDoc = JsonDocument.Parse(json);

        return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
    }

    public static ControllerContext MockControllerContext =>
        new()
        {
            HttpContext = new DefaultHttpContext
            {
                Items = new Dictionary<object, object?>() { { "DeviceAccountId", "id" } }
            }
        };

    public const string DeviceAccountId = "id";

    public static T? GetData<T>(this ActionResult<DragaliaResponse<object>> response)
        where T : class
    {
        DragaliaResponse<object>? innerResponse =
            (response.Result as OkObjectResult)?.Value as DragaliaResponse<object>;
        return innerResponse?.data as T;
    }

    public static async Task<DragaliaResponse<TResponse>> PostMsgpack<TResponse>(
        this HttpClient client,
        string endpoint,
        object request
    ) where TResponse : class
    {
        HttpContent content = CreateMsgpackContent(request);

        HttpResponseMessage response = await client.PostAsync(endpoint, content);

        response.EnsureSuccessStatusCode();

        byte[] body = await response.Content.ReadAsByteArrayAsync();
        return MessagePackSerializer.Deserialize<DragaliaResponse<TResponse>>(
            body,
            CustomResolver.Options
        );
    }

    public static void WriteAsJson(this ITestOutputHelper output, object value)
    {
        output.WriteLine(
            JsonSerializer.Serialize(value, new JsonSerializerOptions() { WriteIndented = true })
        );
    }
}
