using System.Diagnostics;
using DragaliaAPI.Features.Web;
using DragaliaAPI.Features.Web.News;
using DragaliaAPI.Features.Web.Users;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using static DragaliaAPI.Features.Web.AuthConstants;

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
                SchemeNames.WebJwtScheme,
                opts =>
                {
                    opts.Events = new()
                    {
                        OnMessageReceived = WebAuthenticationHelper.OnMessageReceived,
                        OnTokenValidated = WebAuthenticationHelper.OnTokenValidated,
                    };
                    // The rest is configured in ConfigureJwtBearerOptions.cs after the ServiceProvider is built.
                }
            );

        serviceCollection
            .AddAuthorizationBuilder()
            .AddPolicy(
                PolicyNames.RequireValidJwt,
                builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(SchemeNames.WebJwtScheme)
            )
            .AddPolicy(
                PolicyNames.RequireDawnshardIdentity,
                builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(SchemeNames.WebJwtScheme)
                        .RequireAssertion(ctx =>
                            ctx.User.Identities.Any(x => x.Label == IdentityLabels.Dawnshard)
                        )
                        .RequireClaim(CustomClaimType.AccountId)
                        .RequireClaim(CustomClaimType.ViewerId)
            );

        return serviceCollection;
    }
}
