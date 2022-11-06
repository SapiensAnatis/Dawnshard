using DragaliaAPI.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Shared;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureSharedServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharaDataService>(new CharaDataService())
            .AddSingleton<IDragonDataService>(new DragonDataService());
    }
}
