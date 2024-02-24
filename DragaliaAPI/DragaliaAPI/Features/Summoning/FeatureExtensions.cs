using DragaliaAPI.Features.Summoning;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddSummoningFeature(
        this IServiceCollection serviceCollection
    ) =>
        serviceCollection.AddScoped<ISummonService, SummonService>().AddScoped<SummonListService>();
}
