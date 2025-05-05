using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Friends;

public record HelperProjection(
    AtgenSupportChara EquippedChara,
    AtgenSupportDragon? EquippedDragon,
    AtgenSupportWeaponBody? EquippedWeaponBody,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType1Crest1,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType1Crest2,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType1Crest3,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType2Crest1,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType2Crest2,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType3Crest1,
    AtgenSupportCrestSlotType1List? EquippedCrestSlotType3Crest2,
    AtgenSupportTalisman? EquippedTalisman,
    UserDataProjection UserData,
    int? ReliabilityLevel,
    int? PartyPower
);

public record UserDataProjection(
    long ViewerId,
    string Name,
    int Level,
    DateTimeOffset LastLoginDate,
    Emblems EmblemId
);
