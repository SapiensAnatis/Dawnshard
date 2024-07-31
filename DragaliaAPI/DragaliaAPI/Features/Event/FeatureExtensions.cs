using DragaliaAPI.Features.Event;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddEventFeature(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<IEventDropService, EventDropService>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<EventValidationFilter>();
}
