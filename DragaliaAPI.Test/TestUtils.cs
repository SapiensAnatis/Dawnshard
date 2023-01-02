﻿using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using FluentAssertions.Equivalency;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Xunit.Abstractions;

namespace DragaliaAPI.Test;

public static class TestUtils
{
    static TestUtils()
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText("RSA_key.pem").AsSpan());
        SecurityKeys.Add(new RsaSecurityKey(rsa) { KeyId = "key" });
    }

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
    /// Consistent security key.
    /// </summary>
    public static IList<SecurityKey> SecurityKeys { get; } = new List<SecurityKey>();

    /// <summary>
    /// Consistent account id to be used in setups for unit tests.
    /// This is also what the user is authenticated as in the mock controller context.
    /// </summary>
    public const string DeviceAccountId = "id";

    /// <summary>
    /// Cast the data of a <see cref="ActionResult{DragaliaResponse}"/> to a given type.
    /// <remarks>Uses 'as' casting, and will return null if the cast failed.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of response.data which the cast should yield.</typeparam>
    /// <param name="response">The ActionResult response from the controller</param>
    /// <returns>The inner data.</returns>
    public static T? GetData<T>(this ActionResult<DragaliaResponse<object>> response)
        where T : class
    {
        DragaliaResponse<object>? innerResponse =
            (response.Result as OkObjectResult)?.Value as DragaliaResponse<object>;
        return innerResponse?.data as T;
    }

    /// <summary>
    /// Helper to post a msgpack request and deserialize the inner response data.
    /// </summary>
    /// <typeparam name="TResponse">The inner response data type</typeparam>
    /// <param name="client">HTTP client</param>
    /// <param name="endpoint">Endpoint to POST to</param>
    /// <param name="request">Request object to send</param>
    /// <returns></returns>
    public static async Task<DragaliaResponse<TResponse>> PostMsgpack<TResponse>(
        this HttpClient client,
        string endpoint,
        object request,
        bool ensureSuccessHeader = true
    ) where TResponse : class
    {
        HttpContent content = CreateMsgpackContent(request);

        HttpResponseMessage response = await client.PostAsync(endpoint, content);

        response.EnsureSuccessStatusCode();

        byte[] body = await response.Content.ReadAsByteArrayAsync();
        var deserialized = MessagePackSerializer.Deserialize<DragaliaResponse<TResponse>>(
            body,
            CustomResolver.Options
        );

        if (ensureSuccessHeader)
            deserialized.data_headers.result_code.Should().Be(ResultCode.SUCCESS);

        return deserialized;
    }

    /// <summary>
    /// Post a msgpack request, but do not attempt to deserialize it.
    /// Used for checking cases that should return non-200 codes.
    /// </summary>
    /// <param name="client">HTTP client.</param>
    /// <param name="endpoint">The endpoint to POST to.</param>
    /// <param name="request">The request to send.</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> PostMsgpackBasic(
        this HttpClient client,
        string endpoint,
        object request
    )
    {
        HttpContent content = CreateMsgpackContent(request);

        return await client.PostAsync(endpoint, content);
    }

    /// <summary>
    /// Add a <see cref="CustomClaimType.AccountId"/> to the controller's User, allowing the lookup of account ID.
    /// Uses the TestFixture's <see cref="DeviceAccountId"/>.
    /// </summary>
    /// <param name="controller"></param>
    public static void SetupMockContext(this DragaliaControllerBase controller)
    {
        controller.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new(
                    new ClaimsIdentity(
                        new List<Claim>() { new Claim(CustomClaimType.AccountId, DeviceAccountId) }
                    )
                )
            }
        };
    }
}
