using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.AbilityCrest;

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
