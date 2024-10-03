using System.Net;
using System.Net.Http.Headers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Api;

internal sealed partial class BaasApi : IBaasApi
{
    private readonly HttpClient client;
    private readonly IDistributedCache cache;
    private readonly ILogger<BaasApi> logger;

    private const string KeySetEndpoint = "/.well-known/jwks.json";
    private const string SavefileEndpoint = "/gameplay/v1/savefile";
    private const string RedisKey = ":jwks:baas";

    public BaasApi(HttpClient client, IDistributedCache cache, ILogger<BaasApi> logger)
    {
        this.client = client;
        this.cache = cache;
        this.logger = logger;
    }

    public async Task<IList<SecurityKey>> GetKeys()
    {
        string? cachedKeys = await cache.GetStringAsync(RedisKey);
        if (!string.IsNullOrEmpty(cachedKeys))
        {
            JsonWebKeySet cachedJwks = new(cachedKeys);
            return cachedJwks.GetSigningKeys();
        }

        HttpResponseMessage keySetResponse = await client.GetAsync(KeySetEndpoint);

        if (!keySetResponse.IsSuccessStatusCode)
        {
            Log.ReceivedNon200Response(this.logger, KeySetEndpoint, keySetResponse.StatusCode);

            throw new DragaliaException(
                ResultCode.CommonAuthError,
                "Received failure response from BaaS"
            );
        }

        string response = await keySetResponse.Content.ReadAsStringAsync();
        await cache.SetStringAsync(RedisKey, response);

        JsonWebKeySet jwks = new(response);
        return jwks.GetSigningKeys();
    }

    public async Task<LoadIndexResponse> GetSavefile(string gameIdToken)
    {
        HttpResponseMessage savefileResponse = await client.PostAsJsonAsync<object>(
            SavefileEndpoint,
            new { idToken = gameIdToken }
        );

        if (!savefileResponse.IsSuccessStatusCode)
        {
            Log.ReceivedNon200Response(this.logger, SavefileEndpoint, savefileResponse.StatusCode);

            throw new DragaliaException(
                ResultCode.TransitionLinkedDataNotFound,
                "Received failure response from BaaS"
            );
        }

        return (
                await savefileResponse.Content.ReadFromJsonAsync<
                    DragaliaResponse<LoadIndexResponse>
                >(ApiJsonOptions.Instance)
            )?.Data ?? throw new JsonException("Deserialized savefile was null");
    }

    public async Task<string?> GetUserId(string webIdToken)
    {
        UserIdResponse? userInfo = await this.GetUserInformation<UserIdResponse>(
            "/gameplay/v1/user",
            webIdToken
        );

        return userInfo?.UserId;
    }

    public async Task<string?> GetUsername(string gameIdToken)
    {
        WebUserResponse? webUserInfo = await this.GetUserInformation<WebUserResponse>(
            "/gameplay/v1/webUser",
            gameIdToken
        );

        return webUserInfo?.Username;
    }

    private async Task<TResponse?> GetUserInformation<TResponse>(
        string endpoint,
        string bearerToken
    )
        where TResponse : class
    {
        HttpRequestMessage request =
            new(HttpMethod.Get, endpoint)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", bearerToken) },
            };

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Log.ReceivedUserInfo404Response(this.logger, endpoint);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            Log.ReceivedNon200Response(this.logger, endpoint, response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    private class UserIdResponse
    {
        public required string UserId { get; init; }
    }

    private class WebUserResponse
    {
        public required string Username { get; init; }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Received non-200 status code from {Endpoint}: {Status}")]
        public static partial void ReceivedNon200Response(
            ILogger logger,
            string endpoint,
            HttpStatusCode status
        );

        [LoggerMessage(
            LogLevel.Information,
            "Failed to get user information from {Endpoint}: BaaS returned 404 Not Found"
        )]
        public static partial void ReceivedUserInfo404Response(ILogger logger, string endpoint);
    }
}
