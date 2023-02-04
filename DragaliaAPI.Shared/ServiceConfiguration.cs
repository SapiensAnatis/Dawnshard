using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Shared;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureSharedServices(
        this IServiceCollection serviceCollection
    )
    {
        return serviceCollection.AddScoped<IPlayerDetailsService, PlayerDetailsService>();
    }
}
