using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Missions;

public interface IMissionProgressionService
{
    void OnFortPlantUpgraded(FortPlants plant);
    void OnFortPlantBuilt(FortPlants plant);
    void OnFortLevelup();
    void OnQuestCleared(int questId);
    void OnVoidBattleCleared();
    void OnWeaponEarned(UnitElement element, int stars, WeaponSeries series);
    void OnWeaponRefined(UnitElement element, int stars, WeaponSeries series);
    void OnWyrmprintAugmentBuildup(PlusCountType type);
    void OnCharacterBuildup(PlusCountType type);
    void OnItemSummon();

    Task ProcessMissionEvents();
}
