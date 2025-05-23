using DragaliaAPI.Features.Web;
using DragaliaAPI.Features.Web.News;
using DragaliaAPI.Features.Web.Savefile;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Features.Web.TimeAttack;
using DragaliaAPI.Features.Web.Users;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.IdentityModel.Logging;
using static DragaliaAPI.Infrastructure.Authentication.AuthConstants;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddWebFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<UserService>()
            .AddScoped<NewsService>()
            .AddScoped<SavefileEditService>()
            .AddScoped<TimeAttackService>()
            .AddScoped<SettingsService>();

        serviceCollection
            .AddAuthentication()
            .AddJwtBearer(
                SchemeNames.WebJwt,
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
                PolicyNames.RequireValidWebJwt,
                builder =>
                    builder.RequireAuthenticatedUser().AddAuthenticationSchemes(SchemeNames.WebJwt)
            )
            .AddPolicy(
                PolicyNames.RequireDawnshardIdentity,
                builder =>
                    builder
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(SchemeNames.WebJwt)
                        .RequireAssertion(ctx =>
                            ctx.User.Identities.Any(x => x.Label == IdentityLabels.Dawnshard)
                        )
                        .RequireClaim(CustomClaimType.AccountId)
                        .RequireClaim(CustomClaimType.ViewerId)
            );

        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;

        return serviceCollection;
    }
}
