using DragaliaAPI.Features.Zena;
using DragaliaAPI.Infrastructure.Authentication;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddZenaFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IZenaService, ZenaService>();

        serviceCollection.AddAuthentication(opts =>
            opts.AddScheme<ZenaAuthenticationHandler>(AuthConstants.SchemeNames.Zena, null)
        );

        return serviceCollection;
    }
}
