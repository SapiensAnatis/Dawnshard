using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Friends;

public record HelperProjection(
    DbPlayerCharaData EquippedChara,
    DbPlayerDragonData? EquippedDragon,
    DbWeaponBody? EquippedWeaponBody,
    DbAbilityCrest? EquippedCrestSlotType1Crest1,
    DbAbilityCrest? EquippedCrestSlotType1Crest2,
    DbAbilityCrest? EquippedCrestSlotType1Crest3,
    DbAbilityCrest? EquippedCrestSlotType2Crest1,
    DbAbilityCrest? EquippedCrestSlotType2Crest2,
    DbAbilityCrest? EquippedCrestSlotType3Crest1,
    DbAbilityCrest? EquippedCrestSlotType3Crest2,
    DbTalisman? EquippedTalisman,
    DbPlayerUserData UserData,
    DbPlayerDragonReliability? Reliability,
    DbPartyPower? PartyPower
);
