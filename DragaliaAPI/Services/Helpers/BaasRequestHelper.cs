using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Helpers;

public class BaasRequestHelper : IBaasRequestHelper
{
    private readonly IOptionsMonitor<DragaliaAuthOptions> options;
    private readonly HttpClient client;
    private readonly ILogger<BaasRequestHelper> logger;
    private const string KeySetEndpoint = "/.well-known/jwks.json";

    public BaasRequestHelper(
        IOptionsMonitor<DragaliaAuthOptions> options,
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
        HttpResponseMessage keySetResponse = await this.client.GetAsync(
            this.options.CurrentValue.BaasUrl + KeySetEndpoint
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

        return jwks.GetSigningKeys();
    }
}
