using DragaliaAPI.Features.Missions.InitialProgress;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public static class MissionServiceRegistration
{
    public static IServiceCollection RegisterMissionServices(
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection
            .AddScoped<IMissionRepository, MissionRepository>()
            .AddScoped<IMissionService, MissionService>()
            .AddScoped<IMissionProgressionService, MissionProgressionService>()
            .AddScoped<IMissionInitialProgressionService, MissionInitialProgressionService>()
            .AddScoped<IFortMissionProgressionService, FortMissionProgressionService>()
            .AddScoped<FortDataService>();

        serviceCollection.AddKeyedScoped<IInitialProgressCalculator, WallCalculator>(
            MissionCompleteType.WallLevelCleared
        );

        return serviceCollection;
    }
}
