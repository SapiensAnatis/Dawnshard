using DragaliaAPI.Features.Wall;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddWallFeature(this IServiceCollection serviceCollection) =>
        serviceCollection.AddScoped<IWallService, WallService>();
}
