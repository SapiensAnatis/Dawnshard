using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionProgressionService
{
    // TODO: Maybe remove these and make the service that deals with quest play counts supply the events directly?
    void OnQuestCleared(int questId, int count, int total);
    void OnQuestGroupCleared(int questGroupId, int count, int total);

    void OnQuestStoryCleared(int id);

    void OnWeaponEarned(WeaponBodies weapon, UnitElement element, int stars, WeaponSeries series);
    void OnWeaponRefined(
        int count,
        int total,
        WeaponBodies weapon,
        UnitElement element,
        int stars,
        WeaponSeries series
    );

    void OnAbilityCrestBuildupPlusCount(
        AbilityCrests crest,
        PlusCountType type,
        int count,
        int total
    );
    void OnAbilityCrestLevelUp(AbilityCrests crest, int count, int totalLevel);

    void OnCharacterBuildupPlusCount(Charas chara, PlusCountType type, int count, int total);
    void OnCharacterLevelUp(Charas chara, int count, int totalLevel);

    void OnItemSummon();

    void EnqueueEvent(
        MissionCompleteType type,
        int value = 1,
        int total = 1,
        int? parameter = null,
        int? parameter2 = null,
        int? parameter3 = null,
        int? parameter4 = null
    );

    Task ProcessMissionEvents();
}
