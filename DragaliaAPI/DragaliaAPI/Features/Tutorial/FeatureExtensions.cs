using DragaliaAPI.Features.Tutorial;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddTutorialFeature(
        this IServiceCollection serviceCollection
    ) => serviceCollection.AddScoped<ITutorialService, TutorialService>();
}
