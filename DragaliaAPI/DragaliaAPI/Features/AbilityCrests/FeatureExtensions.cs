using DragaliaAPI.Features.AbilityCrests;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddAbilityCrestFeature(
        this IServiceCollection serviceCollection
    ) =>
        serviceCollection
            .AddScoped<IAbilityCrestRepository, AbilityCrestRepository>()
            .AddScoped<IAbilityCrestService, AbilityCrestService>();
}
