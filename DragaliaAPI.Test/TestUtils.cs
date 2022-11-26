using System.Net.Http.Headers;
using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using FluentAssertions.Equivalency;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

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

    /// <summary>
    /// For a HttpResponse with a msgpack body:
    /// 1. Checks it has a success status code
    /// 2. Deserializes it into the model type representing the body
    /// 3. Checks the deserialization result matches an expected value
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected in the body.</typeparam>
    /// <param name="response">The received response.</param>
    /// <param name="expectedResponse">The expected response body object.</param>
    public static async Task CheckMsgpackResponse<TResponse>(
        HttpResponseMessage response,
        TResponse expectedResponse,
        Func<
            EquivalencyAssertionOptions<TResponse>,
            EquivalencyAssertionOptions<TResponse>
        >? config = null
    )
    {
        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        TResponse? deserializedResponse = MessagePackSerializer.Deserialize<TResponse>(
            responseBytes,
            ContractlessStandardResolver.Options
        );

        config ??= options => options;
        deserializedResponse.Should().BeEquivalentTo(expectedResponse, config);
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
        return MessagePackSerializer.Deserialize<DragaliaResponse<TResponse>>(body);
    }
}
