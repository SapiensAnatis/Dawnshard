namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public enum MissionCompleteType
{
    // Fort
    /// <summary>
    /// FortPlants plantId
    /// </summary>
    FortPlantLevelUp,

    /// <summary>
    /// FortPlants plantId
    /// </summary>
    FortPlantBuilt,

    /// <summary>
    /// FortPlants plantId
    /// </summary>
    FortPlantPlaced,

    /// <summary>
    /// None
    /// </summary>
    FortLevelUp,

    // Quest
    /// <summary>
    /// int questId, int questGroupId, QuestPlayModeTypes playMode
    /// </summary>
    QuestCleared,

    /// <summary>
    /// int groupId, VariationTypes type, QuestPlayModeTypes playMode
    /// </summary>
    EventGroupCleared,

    // Story
    /// <summary>
    /// int storyId
    /// </summary>
    QuestStoryCleared,

    // Unit
    /// <summary>
    /// WeaponBodies bodyId, UnitElement element, int rarity, WeaponSeries series
    /// </summary>
    WeaponEarned,

    /// <summary>
    /// WeaponBodies bodyId, UnitElement element, int rarity, WeaponSeries series
    /// </summary>
    WeaponRefined,

    /// <summary>
    /// AbilityCrests crestId, PlusCountType type
    /// </summary>
    AbilityCrestBuildupPlusCount,

    /// <summary>
    /// AbilityCrests crestId
    /// </summary>
    AbilityCrestTotalPlusCountUp, // For missions that are "Upgrade a Wyrmprint's HP & Strength X Times"

    /// <summary>
    /// AbilityCrests crestId
    /// </summary>
    AbilityCrestLevelUp,

    /// <summary>
    /// Charas charaId, UnitElement element, PlusCountType type
    /// </summary>
    CharacterBuildupPlusCount,

    /// <summary>
    /// Charas charaId, UnitElement element
    /// </summary>
    CharacterLevelUp,

    /// <summary>
    /// Charas chara, UnitElement element
    /// </summary>
    CharacterManaNodeUnlock,

    // Dragon
    /// <summary>
    /// Dragons dragon, UnitElement element
    /// </summary>
    DragonLevelUp,

    /// <summary>
    /// Dragons dragon, DragonGifts gift, UnitElement element
    /// </summary>
    DragonGiftSent,

    /// <summary>
    /// Dragons dragon, UnitElement element
    /// </summary>
    DragonBondLevelUp,

    // Misc
    /// <summary>
    /// None
    /// </summary>
    ItemSummon,

    /// <summary>
    /// None
    /// </summary>
    PlayerLevelUp,

    /// <summary>
    /// None
    /// </summary>
    AccountLinked,

    /// <summary>
    /// UnitElement element
    /// </summary>
    PartyOptimized,

    /// <summary>
    /// None
    /// </summary>
    AbilityCrestTradeViewed,

    /// <summary>
    /// None (Unimplemented)
    /// </summary>
    GuildCheckInRewardClaimed,

    /// <summary>
    /// None
    /// </summary>
    PartyPowerReached,

    /// <summary>
    /// int tradeId, EntityTypes type, int id
    /// </summary>
    TreasureTrade,

    /// <summary>
    /// None
    /// </summary>
    UnimplementedAutoComplete = 99999, // Auto-completes upon receiving
}
