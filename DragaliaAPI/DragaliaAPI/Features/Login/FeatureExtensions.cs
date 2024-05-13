using DragaliaAPI.Features.Login;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddLoginFeature(this IServiceCollection serviceCollection) =>
        serviceCollection.AddScoped<ILoginService, LoginService>();
}
