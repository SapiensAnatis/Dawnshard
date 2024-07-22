using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Event.Summon;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddEventFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IEventDropService, EventDropService>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<EventSummonService>()
            .AddScoped<EventValidationFilter>()
            .AddSingleton<TwoStepItemGenerator, VoidBattleItemGenerator>();

        serviceCollection.AddOptions<EventSummonOptions>().BindConfiguration(string.Empty);

        return serviceCollection;
    }
}
