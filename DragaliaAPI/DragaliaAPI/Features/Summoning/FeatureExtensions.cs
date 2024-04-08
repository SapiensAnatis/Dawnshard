using DragaliaAPI.Features.Summoning;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddSummoningFeature(
        this IServiceCollection serviceCollection
    ) => serviceCollection.AddScoped<SummonService>().AddScoped<SummonOddsService>();

    public static IServiceCollection AddSummoningOptions(
        this IServiceCollection serviceCollection,
        IConfiguration config
    )
    {
        serviceCollection
            .Configure<SummonBannerOptions>(config.GetRequiredSection(nameof(SummonBannerOptions)))
            .AddOptions<SummonBannerOptions>()
            .PostConfigure(opts => opts.PostConfigure());

        return serviceCollection;
    }
}
