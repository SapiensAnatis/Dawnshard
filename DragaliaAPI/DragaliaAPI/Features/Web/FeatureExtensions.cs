using System.IdentityModel.Tokens.Jwt;
using DragaliaAPI.Features.Web;
using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddWebFeature(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        BaasOptions baasOptions =
            configuration.GetRequiredSection("Baas").Get<BaasOptions>()
            ?? throw new InvalidOperationException("Failed to load BaasOptions");

    serviceCollection
        .AddAuthentication()
        .AddJwtBearer(
            WebAuthenticationHelper.PolicyName,
            opts =>
            {
                opts.Audience = baasOptions.TokenAudience;
                opts.Events = new()
                {
                    OnMessageReceived = WebAuthenticationHelper.OnMessageReceived,
                    OnTokenValidated = WebAuthenticationHelper.OnTokenValidated
                };
                opts.Configuration = new()
                {
                    JwksUri = new Uri(baasOptions.BaasUrlParsed, "/.well-known/jwks.json").AbsoluteUri,
                    Issuer = baasOptions.TokenIssuer
                };
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
