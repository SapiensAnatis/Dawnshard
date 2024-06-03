using DragaliaAPI.Features.Summoning;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddSummoningFeature(
        this IServiceCollection serviceCollection
    ) =>
        serviceCollection
            .AddScoped<SummonService>()
            .AddScoped<SummonOddsService>()
            .AddScoped<UnitService>();

    public static IServiceCollection AddSummoningOptions(
        this IServiceCollection serviceCollection,
        IConfiguration config
    )
    {
        serviceCollection
            .Configure<SummonBannerOptions>(config.GetRequiredSection(nameof(SummonBannerOptions)))
            .AddOptions<SummonBannerOptions>()
            .Validate(
                opts => opts.Banners.DistinctBy(x => x.Id).Count() == opts.Banners.Count,
                "bannerConfig.json IDs must be unique!"
            )
            .ValidateOnStart()
            .PostConfigure(opts => opts.PostConfigure());

        return serviceCollection;
    }
}
