using DragaliaAPI.Features.Album;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddAlbumFeature(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<AlbumService, AlbumService>();
    }
}
