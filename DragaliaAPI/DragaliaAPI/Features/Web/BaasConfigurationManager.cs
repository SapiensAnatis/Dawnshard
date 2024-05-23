using DragaliaAPI.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Features.Web;

public class BaasConfigurationManager(IHttpClientFactory clientFactory)
    : IConfigurationManager<OpenIdConnectConfiguration>
{
    private OpenIdConnectConfiguration? configuration;

    public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
    {
        if (this.configuration is not null)
        {
            return Task.FromResult(this.configuration);
        }

        return this.LoadConfigurationAsync(cancel);
    }

    private async Task<OpenIdConnectConfiguration> LoadConfigurationAsync(CancellationToken cancel)
    {
        HttpClient client = clientFactory.CreateClient(nameof(BaasConfigurationManager));

        HttpResponseMessage keySetResponse = await client.GetAsync(
            "/.well-known/jwks.json",
            cancel
        );

        if (!keySetResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to retrieve BaaS signing keys: status {keySetResponse.StatusCode}"
            );
        }

        string response = await keySetResponse.Content.ReadAsStringAsync(cancel);

        this.configuration = new OpenIdConnectConfiguration();

        foreach (SecurityKey key in JsonWebKeySet.Create(response).GetSigningKeys())
        {
            this.configuration.SigningKeys.Add(key);
        }

        return this.configuration;
    }

    public void RequestRefresh()
    {
        // No-op. Concerned about re-fetching configuration too many times considering the slightly cursed impl of 
        // LoadConfigurationAsync.
    }
}
