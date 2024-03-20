using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public class FortMissionProgressionService(
    FortDataService fortDataService,
    IMissionProgressionService missionProgressionService
) : IFortMissionProgressionService
{
    public async Task OnFortPlantBuild(FortPlants plant)
    {
        int currentCount = await fortDataService.GetFortPlantCount(plant);

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortPlantBuilt,
            1,
            currentCount + 1,
            (int)plant
        );

        int currentFortLevel = await fortDataService.GetTotalFortLevel();

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortLevelUp,
            1,
            currentFortLevel + 1
        );
    }

    public async Task OnFortPlantLevelUp(FortPlants plant, int level)
    {
        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortPlantLevelUp,
            total: level,
            parameter: (int)plant
        );

        int currentFortLevel = await fortDataService.GetTotalFortLevel();

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortLevelUp,
            total: currentFortLevel + 1
        );
    }

    public async Task OnFortPlantPlace(FortPlants plant)
    {
        int currentCount = await fortDataService.GetFortPlantCount(plant);

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortPlantPlaced,
            total: currentCount + 1,
            parameter: (int)plant
        );
    }

    public void OnFortIncomeCollected(EntityTypes type) =>
        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortIncomeCollected,
            1,
            1,
            parameter: (int)type
        );
}
