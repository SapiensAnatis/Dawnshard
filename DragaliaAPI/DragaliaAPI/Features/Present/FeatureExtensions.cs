using DragaliaAPI.Features.Present;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddPresentFeature(this IServiceCollection services) =>
        services
            .AddScoped<IPresentService, PresentService>()
            .AddScoped<IPresentControllerService, PresentControllerService>();
}
