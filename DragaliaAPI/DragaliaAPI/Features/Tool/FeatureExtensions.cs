using DragaliaAPI.Features.Tool;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Services.Game;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddToolFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthService, AuthService>();

        serviceCollection
            .AddAuthentication()
            .AddJwtBearer(
                AuthConstants.SchemeNames.GameJwt,
                options =>
                {
                    options.Events = new()
                    {
                        OnMessageReceived = ToolAuthenticationHelper.OnMessageReceived,
                    };
                    // The rest is configured in ConfigureJwtBearerOptions.cs after the ServiceProvider is built.
                }
            );

        return serviceCollection;
    }
}
