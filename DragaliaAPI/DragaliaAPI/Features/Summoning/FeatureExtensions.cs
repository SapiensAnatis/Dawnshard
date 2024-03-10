using DragaliaAPI.Features.Summoning;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddSummoningFeature(
        this IServiceCollection serviceCollection
    ) =>
        serviceCollection
            .AddScoped<ISummonService, SummonService>()
            .AddScoped<SummonListService>()
            .AddScoped<SummonOddsService>();

    public static IServiceCollection AddSummoningOptions(
        this IServiceCollection serviceCollection,
        IConfiguration config
    )
    {
        serviceCollection
            .Configure<SummonBannerOptions>(config.GetRequiredSection(nameof(SummonBannerOptions)))
            .AddOptions<SummonBannerOptions>()
            .PostConfigure(opts =>
            {
                opts.Banners.Add(
                    new Banner()
                    {
                        Id = SummonConstants.RedoableSummonBannerId,
                        IsGala = true,
                        Start = DateTimeOffset.MinValue,
                        End = DateTimeOffset.MaxValue,
                    }
                );
            });

        return serviceCollection;
    }
}
