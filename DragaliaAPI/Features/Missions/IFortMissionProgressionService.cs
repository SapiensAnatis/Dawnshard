using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IFortMissionProgressionService
{
    Task<int> GetTotalFortLevel();
    Task<int> GetMaxFortPlantLevel(FortPlants plant);
    Task<int> GetFortPlantCount(FortPlants plant);

    Task OnFortPlantBuild(FortPlants plant);
    Task OnFortPlantLevelUp(FortPlants plant, int level);
    Task OnFortPlantPlace(FortPlants plant);
}
