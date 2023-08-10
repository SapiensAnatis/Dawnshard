namespace DragaliaAPI.Shared.MasterAsset.Models;

public record UnionAbility(
    int Id,
    int CrestGroup1Count1,
    int AbilityId1,
    int PartyPower1,
    int CrestGroup1Count2,
    int AbilityId2,
    int PartyPower2,
    int CrestGroup1Count3,
    int AbilityId3,
    int PartyPower3,
    int CrestGroup1Count4,
    int AbilityId4,
    int PartyPower4,
    int CrestGroup1Count5,
    int AbilityId5,
    int PartyPower5
)
{
    public readonly (int Count, int AbilityId, int Power)[] Abilities =
    {
        (CrestGroup1Count1, AbilityId1, PartyPower1),
        (CrestGroup1Count2, AbilityId2, PartyPower2),
        (CrestGroup1Count3, AbilityId3, PartyPower3),
        (CrestGroup1Count4, AbilityId4, PartyPower4),
        (CrestGroup1Count5, AbilityId5, PartyPower5)
    };
};
