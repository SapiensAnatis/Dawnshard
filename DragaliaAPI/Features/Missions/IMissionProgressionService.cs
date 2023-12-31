using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionProgressionService
{
    // TODO: Maybe remove these and make the service that deals with quest play counts supply the events directly?
    void OnQuestCleared(
        int questId,
        int questGroupId,
        QuestPlayModeTypes playMode,
        int count,
        int total
    );

    void OnEventGroupCleared(
        int eventGroupId,
        VariationTypes type,
        QuestPlayModeTypes playMode,
        int count,
        int total
    );

    void OnQuestStoryCleared(int id);

    void OnWeaponEarned(WeaponBodies weapon, UnitElement element, int rarity, WeaponSeries series);

    void OnWeaponRefined(
        int count,
        int total,
        WeaponBodies weapon,
        UnitElement element,
        int rarity,
        WeaponSeries series
    );

    void OnAbilityCrestBuildupPlusCount(
        AbilityCrests crest,
        PlusCountType type,
        int count,
        int total
    );

    void OnAbilityCrestTotalPlusCountUp(AbilityCrests crest, int count, int total);
    void OnAbilityCrestLevelUp(AbilityCrests crest, int count, int totalLevel);

    void OnCharacterBuildupPlusCount(
        Charas chara,
        UnitElement element,
        PlusCountType type,
        int count,
        int total
    );

    void OnCharacterLevelUp(Charas chara, UnitElement element, int count, int totalLevel);
    void OnCharacterManaNodeUnlock(Charas chara, UnitElement element, int count, int total);
    void OnDragonLevelUp(Dragons dragon, UnitElement element, int count, int total);

    void OnDragonGiftSent(
        Dragons dragon,
        DragonGifts gift,
        UnitElement element,
        int count,
        int total
    );

    void OnDragonBondLevelUp(Dragons dragon, UnitElement element, int levelDiff, int newLevel);
    void OnItemSummon();
    void OnPartyOptimized(UnitElement element);
    void OnAbilityCrestTradeViewed();
    void OnGuildCheckInRewardClaimed();
    void OnPartyPowerReached(int might);
    void OnTreasureTrade(int tradeId, EntityTypes type, int id, int count, int total);
    void OnEventParticipation(int eventId);
    void OnEventRegularBattleCleared(int eventId, VariationTypes variationType);
    void OnEventQuestClearedWithCrest(int eventId, AbilityCrests crest);
    void OnEventPointCollected(int eventId, VariationTypes variationType, int quantity);
    void OnEventChallengeBattleCleared(
        int eventId,
        VariationTypes variationType,
        bool fullClear,
        int questId
    );
    void OnEventTrialCleared(int eventId, VariationTypes variationType);

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
