using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
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

    private IList<SecurityKey>? cachedKeys;

    public BaasRequestHelper(
        IOptionsMonitor<BaasOptions> options,
        HttpClient client,
        ILogger<BaasRequestHelper> logger
    )
    {
        this.options = options;
        this.client = client;
        this.logger = logger;
    }

    public async Task<IList<SecurityKey>> GetKeys()
    {
        if (this.cachedKeys is not null)
        {
            return cachedKeys;
        }

        HttpResponseMessage keySetResponse = await this.client.GetAsync(
            new Uri(this.options.CurrentValue.BaasUrlParsed, KeySetEndpoint)
        );

        if (!keySetResponse.IsSuccessStatusCode)
        {
            logger.LogError("Received failure response from BaaS: {@response}", keySetResponse);

            throw new DragaliaException(
                Models.ResultCode.COMMON_AUTH_ERROR,
                "Received failure response from BaaS"
            );
        }

        JsonWebKeySet jwks = new(await keySetResponse.Content.ReadAsStringAsync());

        cachedKeys = jwks.GetSigningKeys();
        return cachedKeys;
    }

    public async Task<LoadIndexData> GetSavefile(string idToken)
    {
        HttpResponseMessage savefileResponse = await this.client.PostAsJsonAsync<object>(
            new Uri(this.options.CurrentValue.BaasUrlParsed, SavefileEndpoint),
            new { idToken }
        );

        if (!savefileResponse.IsSuccessStatusCode)
        {
            logger.LogError("Received failure response from BaaS: {@response}", savefileResponse);

            throw new DragaliaException(
                Models.ResultCode.TRANSITION_LINKED_DATA_NOT_FOUND,
                "Received failure response from BaaS"
            );
        }

        return await savefileResponse.Content.ReadFromJsonAsync<LoadIndexData>()
            ?? throw new NullReferenceException("Received null savefile from response");
    }
}
