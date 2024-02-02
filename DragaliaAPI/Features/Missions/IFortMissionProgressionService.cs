using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IFortMissionProgressionService
{
    Task OnFortPlantBuild(FortPlants plant);
    Task OnFortPlantLevelUp(FortPlants plant, int level);
    Task OnFortPlantPlace(FortPlants plant);
    void OnFortIncomeCollected(EntityTypes type);
}
