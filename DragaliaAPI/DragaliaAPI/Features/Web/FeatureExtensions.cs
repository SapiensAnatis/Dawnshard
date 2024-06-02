using DragaliaAPI.Features.Web;
using DragaliaAPI.Features.Web.News;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddWebFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>()
            .AddScoped<UserService>()
            .AddScoped<NewsService>();

        serviceCollection
            .AddAuthentication()
            .AddJwtBearer(
                WebAuthenticationHelper.SchemeName,
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
                        .AddAuthenticationSchemes(WebAuthenticationHelper.SchemeName)
            );

        return serviceCollection;
    }
}
