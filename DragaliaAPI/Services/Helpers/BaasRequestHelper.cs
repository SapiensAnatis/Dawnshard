using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Helpers;

public class BaasRequestHelper : IBaasRequestHelper
{
    private readonly IOptionsMonitor<BaasOptions> options;
    private readonly HttpClient client;
    private readonly ILogger<BaasRequestHelper> logger;
    private const string KeySetEndpoint = "/.well-known/jwks.json";
    private const string SavefileEndpoint = "/gameplay/v1/savefile";

    private static IList<SecurityKey>? CachedKeys;

    public BaasRequestHelper(
        IOptionsMonitor<BaasOptions> options,
        HttpClient client,
        ILogger<BaasRequestHelper> logger
    )
    {
        this.options = options;
        this.client = client;
        this.logger = logger;

        this.client.BaseAddress = this.options.CurrentValue.BaasUrlParsed;
    }

    public async Task<IList<SecurityKey>> GetKeys()
    {
        if (CachedKeys is not null)
        {
            return CachedKeys;
        }

        HttpResponseMessage keySetResponse = await this.client.GetAsync(KeySetEndpoint);

        if (!keySetResponse.IsSuccessStatusCode)
        {
            logger.LogError("Received failure response from BaaS: {@response}", keySetResponse);

            throw new DragaliaException(
                ResultCode.CommonAuthError,
                "Received failure response from BaaS"
            );
        }

        JsonWebKeySet jwks = new(await keySetResponse.Content.ReadAsStringAsync());

        CachedKeys = jwks.GetSigningKeys();
        return CachedKeys;
    }

    public async Task<LoadIndexData> GetSavefile(string idToken)
    {
        HttpResponseMessage savefileResponse = await this.client.PostAsJsonAsync<object>(
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
            )?.data ?? throw new NullReferenceException("Received null savefile from response");
    }
}
