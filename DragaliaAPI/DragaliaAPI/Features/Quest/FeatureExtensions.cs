using DragaliaAPI.Features.Quest;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddQuestFeature(this IServiceCollection services) =>
        services
            .AddScoped<IQuestService, QuestService>()
            .AddScoped<IQuestCacheService, QuestCacheService>()
            .AddScoped<IQuestTreasureService, QuestTreasureService>();
}
