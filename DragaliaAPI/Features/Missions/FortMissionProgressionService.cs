using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class FortMissionProgressionService(
    IFortRepository fortRepository,
    IMissionProgressionService missionProgressionService
) : IFortMissionProgressionService
{
    public async Task<int> GetTotalFortLevel()
    {
        return await fortRepository
                .Builds.Where(x => x.PlantId != FortPlants.TheHalidom)
                .SumAsync(
                    x => x.BuildEndDate == DateTimeOffset.UnixEpoch ? x.Level : (int?)x.Level - 1
                ) ?? 0;
    }

    public async Task<int> GetMaxFortPlantLevel(FortPlants plant)
    {
        return await fortRepository
                .Builds.Where(x => x.PlantId == plant)
                .Select(x => (int?)x.Level)
                .MaxAsync() ?? 0;
    }

    public async Task<int> GetFortPlantCount(FortPlants plant)
    {
        return await fortRepository.Builds.CountAsync(
            x =>
                x.PlantId == plant
                && x.PositionX != -1 // not in storage
                && x.PositionZ != -1
        );
    }

    public async Task OnFortPlantBuild(FortPlants plant)
    {
        int currentCount = await GetFortPlantCount(plant);

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortPlantBuilt,
            1,
            currentCount + 1,
            (int)plant
        );

        int currentFortLevel = await GetTotalFortLevel();

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

        int currentFortLevel = await GetTotalFortLevel();

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.FortLevelUp,
            total: currentFortLevel + 1
        );
    }

    public async Task OnFortPlantPlace(FortPlants plant)
    {
        int currentCount = await GetFortPlantCount(plant);

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
