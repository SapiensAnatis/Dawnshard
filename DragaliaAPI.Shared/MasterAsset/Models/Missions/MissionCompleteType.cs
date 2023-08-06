namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public enum MissionCompleteType
{
    // Fort
    FortPlantLevelUp,
    FortPlantBuilt,
    FortPlantPlaced,
    FortLevelUp,

    // Quest
    QuestCleared,
    QuestGroupCleared,

    // Story
    QuestStoryCleared,

    // Unit
    WeaponEarned,
    WeaponRefined,

    AbilityCrestBuildupPlusCount,
    AbilityCrestLevelUp,

    CharacterBuildupPlusCount,
    CharacterLevelUp,

    // Misc
    ItemSummon,
    PlayerLevelUp,
    AccountLinked
}
