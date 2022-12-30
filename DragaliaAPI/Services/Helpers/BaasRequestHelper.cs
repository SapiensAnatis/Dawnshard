﻿using DragaliaAPI.Models.Options;
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

        cachedKeys = jwks.GetSigningKeys();
        return cachedKeys;
    }
}
