using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IMissionProgressionService
{
    void OnFortPlantUpgraded(FortPlants plant, int level);
    void OnFortPlantBuilt(FortPlants plant);
    void OnFortLevelup(int level);
    void OnQuestCleared(int questId);
    void OnVoidBattleCleared();
    void OnWeaponEarned(UnitElement element, int stars, WeaponSeries series);
    void OnWeaponRefined(UnitElement element, int stars, WeaponSeries series);
    void OnWyrmprintAugmentBuildup(PlusCountType type, int count);
    void OnCharacterBuildup(PlusCountType type, int count);

    Task ProcessMissionEvents();
}
