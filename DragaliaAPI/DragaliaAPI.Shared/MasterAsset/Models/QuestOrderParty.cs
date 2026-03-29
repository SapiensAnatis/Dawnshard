using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// Represents a unit in a fixed party for quests that force you to use a particular team (fixed team quests).
/// </summary>
/// <remarks>
/// This only applies to 3 quests: Fight for Humanity (Xenos boss), Take Your Heart (Caged Desire) and an unnamed quest
/// that is played when first playing Caged Desire.
/// <br />
/// A quest's fixed party (if there is one) is linked to this entity by <see cref="QuestData.QuestOrderPartyGroupId"/>,
/// which links to this record's <see cref="QuestOrderParty.QuestOrderPartyGroupId"/>. This represents a group of units
/// that should be used in a party, while <see cref="Id"/> represents the ID of an individual unit within the party.
/// </remarks>
public record QuestOrderParty(
    int Id,
    int QuestOrderPartyGroupId,
    Charas CharaId,
    int CharaLevel,
    int CharaRarity,
    int CharaHpPlusCount,
    int CharaAttackPlusCount,
    int ReleaseManaCircle,
    DragonId DragonId,
    int DragonLevel,
    int DragonLimitBreakCount,
    int DragonHpPlusCount,
    int DragonAttackPlusCount,
    int WeaponSkinId,
    WeaponBodies WeaponBodyId,
    int WeaponBodyBuildupCount,
    int WeaponBodyLimitBreakCount,
    int WeaponBodyLimitOverCount,
    // Despite supporting wyrmprint details, in reality all the fixed adventurers have no wyrmprints equipped, so
    // we won't bother to deserialize the details.
    AbilityCrestId CrestSlotType1AbilityCrestId1,
    AbilityCrestId CrestSlotType1AbilityCrestId2,
    AbilityCrestId CrestSlotType1AbilityCrestId3,
    AbilityCrestId CrestSlotType2AbilityCrestId1,
    AbilityCrestId CrestSlotType2AbilityCrestId2,
    AbilityCrestId CrestSlotType3AbilityCrestId1,
    AbilityCrestId CrestSlotType3AbilityCrestId2
);
