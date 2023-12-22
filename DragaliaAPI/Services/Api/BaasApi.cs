using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Api;

public class BaasApi : IBaasApi
{
    private readonly IOptionsMonitor<BaasOptions> options;
    private readonly HttpClient client;
    private readonly IDistributedCache cache;
    private readonly ILogger<BaasApi> logger;

    private const string KeySetEndpoint = "/.well-known/jwks.json";
    private const string SavefileEndpoint = "/gameplay/v1/savefile";
    private const string RedisKey = ":jwks:baas";

    public BaasApi(
        IOptionsMonitor<BaasOptions> options,
        HttpClient client,
        IDistributedCache cache,
        ILogger<BaasApi> logger
    )
    {
        this.options = options;
        this.client = client;
        this.cache = cache;
        this.logger = logger;

        this.client.BaseAddress = this.options.CurrentValue.BaasUrlParsed;
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
            logger.LogError("Received failure response from BaaS: {@response}", keySetResponse);

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

    public async Task<LoadIndexData> GetSavefile(string idToken)
    {
        HttpResponseMessage savefileResponse = await client.PostAsJsonAsync<object>(
            SavefileEndpoint,
            new { idToken }
        );

        if (!savefileResponse.IsSuccessStatusCode)
        {
            logger.LogError("Received failure response from BaaS: {@response}", savefileResponse);

            throw new DragaliaException(
                ResultCode.TransitionLinkedDataNotFound,
                "Received failure response from BaaS"
            );
        }

        return (
                await savefileResponse.Content.ReadFromJsonAsync<DragaliaResponse<LoadIndexData>>(
                    ApiJsonOptions.Instance
                )
            )?.data ?? throw new JsonException("Deserialized savefile was null");
    }
}
