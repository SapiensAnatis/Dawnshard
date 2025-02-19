using System.Net.Http.Headers;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Infrastructure.Serialization.MessagePack;
using MessagePack;

namespace DragaliaAPI.Integration.Test;

public static class HttpClientExtensions
{
    /// <summary>
    /// Helper to post a msgpack request and deserialize the inner response data.
    /// </summary>
    /// <typeparam name="TResponse">The inner response data type</typeparam>
    /// <param name="client">HTTP client</param>
    /// <param name="endpoint">Endpoint to POST to</param>
    /// <param name="request">Request object to send</param>
    /// <param name="ensureSuccessHeader">Whether to check for a success response and throw if not successful.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    public static async Task<DragaliaResponse<TResponse>> PostMsgpack<TResponse>(
        this HttpClient client,
        string endpoint,
        object request,
        bool ensureSuccessHeader = true,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        HttpContent content = CreateMsgpackContent(request);

        HttpResponseMessage response = await client.PostAsync(
            endpoint.TrimStart('/'),
            content,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        byte[] body = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        DragaliaResponse<TResponse> deserialized = MessagePackSerializer.Deserialize<
            DragaliaResponse<TResponse>
        >(body, CustomResolver.Options, cancellationToken);

        if (ensureSuccessHeader)
        {
            deserialized.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        }

        return deserialized;
    }

    public static async Task<DragaliaResponse<TResponse>> PostMsgpack<TResponse>(
        this HttpClient client,
        string endpoint,
        bool ensureSuccessHeader = true,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        HttpResponseMessage response = await client.PostAsync(
            endpoint.TrimStart('/'),
            null,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        byte[] body = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        DragaliaResponse<TResponse> deserialized = MessagePackSerializer.Deserialize<
            DragaliaResponse<TResponse>
        >(body, CustomResolver.Options, cancellationToken);

        if (ensureSuccessHeader)
        {
            deserialized.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        }

        return deserialized;
    }

    public static async Task PostMsgpack(
        this HttpClient client,
        string endpoint,
        object request,
        bool ensureSuccessHeader = true,
        CancellationToken cancellationToken = default
    ) =>
        await client.PostMsgpack<object>(endpoint, request, ensureSuccessHeader, cancellationToken);

    /// <summary>
    /// Post a msgpack request, but do not attempt to deserialize it.
    /// Used for checking cases that should return non-200 codes.
    /// </summary>
    /// <param name="client">HTTP this.Client.</param>
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

        return await client.PostAsync(endpoint.TrimStart('/'), content);
    }

    public static HttpContent CreateMsgpackContent(object content)
    {
        ByteArrayContent result = new(MessagePackSerializer.Serialize(content));
        result.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        return result;
    }
}
