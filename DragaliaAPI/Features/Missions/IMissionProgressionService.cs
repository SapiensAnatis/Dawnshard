using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IMissionProgressionService
{
    void OnFortPlantUpgraded(FortPlants plant, int level);
    void OnFortPlantBuilt(FortPlants plant);
    void OnFortLevelup(int level);
    void OnQuestCleared(int questId);
    void OnVoidBattleCleared();
    void OnWeaponEarned(int id, int abilityId);
    void OnWyrmprintUpgraded(int id, int augmentId, int count);

    Task ProcessMissionEvents();
}