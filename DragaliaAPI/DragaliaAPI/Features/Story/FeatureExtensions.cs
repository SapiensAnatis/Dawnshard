using DragaliaAPI.Features.Story;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddStoryFeature(this IServiceCollection services) =>
        services
            .AddScoped<IStoryService, StoryService>()
            .AddScoped<IStoryRepository, StoryRepository>();
}
