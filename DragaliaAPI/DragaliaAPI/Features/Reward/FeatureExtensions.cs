using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Reward.Handlers;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddRewardFeature(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IRewardService, RewardService>()
            .AddAllOfType<IRewardHandler>()
            .AddAllOfType<IBatchRewardHandler>();
}
