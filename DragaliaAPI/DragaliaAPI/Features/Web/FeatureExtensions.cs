using System.IdentityModel.Tokens.Jwt;
using DragaliaAPI.Features.Web;
using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddWebFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(
            nameof(BaasConfigurationManager),
            (provider, client) =>
            {
                BaasOptions baasOptions = provider
                    .GetRequiredService<IOptions<BaasOptions>>()
                    .Value;
                client.BaseAddress = baasOptions.BaasUrlParsed;
            }
        );

        serviceCollection
            .AddSingleton<BaasConfigurationManager>()
            .AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>()
            .AddAuthentication()
            .AddJwtBearer(
                WebAuthenticationHelper.PolicyName,
                opts =>
                {
                    opts.Events = new()
                    {
                        OnMessageReceived = WebAuthenticationHelper.OnMessageReceived,
                        OnTokenValidated = WebAuthenticationHelper.OnTokenValidated
                    };
                    // The rest is configured in ConfigureJwtBearerOptions.cs after the ServiceProvider is built.
                }
            );

        serviceCollection
            .AddAuthorizationBuilder()
            .AddPolicy(
                WebAuthenticationHelper.PolicyName,
                builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(WebAuthenticationHelper.PolicyName)
            );

        return serviceCollection;
    }
}
