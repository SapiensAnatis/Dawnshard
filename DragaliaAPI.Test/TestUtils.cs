using System.Net.Http.Headers;
using System.Text.Json;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using FluentAssertions.Equivalency;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Test;

public static class TestUtils
{
    public static void InitializeCacheForTests(IDistributedCache cache)
    {
        // Downside of making Session a private nested class: I have to type this manually :(
        string preparedSessionJson = """
            {
                "SessionId": "prepared_session_id",
                "DeviceAccountId": "prepared_id"
            }
            """;
        cache.SetString(":session:id_token:id_token", preparedSessionJson);

        string sessionJson = """
                {
                    "SessionId": "session_id",
                    "DeviceAccountId": "logged_in_id"
                }
                """;
        cache.SetString(":session:session_id:session_id", sessionJson);
        cache.SetString(":session_id:device_account_id:logged_in_id", "session_id");
    }

    public static List<DbDeviceAccount> GetDeviceAccountsSeed()
    {
        return new()
        {
            // Password is a hash of the string "password"
            new("id", "NMvdakTznEF6khwWcz17i6GTnDA="),
        };
    }

    public static List<DbPlayerUserData> GetSavefilePlayerInfoSeed()
    {
        return new()
        {
            DbSavefileUserDataFactory.Create("id"),
            DbSavefileUserDataFactory.Create("prepared_id"),
            DbSavefileUserDataFactory.Create("logged_in_id")
        };
    }

    public static HttpContent CreateMsgpackContent(byte[] content, string sessionId = "session_id")
    {
        ByteArrayContent result = new(content);
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
}
