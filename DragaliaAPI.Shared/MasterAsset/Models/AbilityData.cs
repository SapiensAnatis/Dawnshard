using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityData(
    int Id,
    AbilityTypes AbilityType1,
    double AbilityType1UpValue,
    int AbilityLimitedGroupId1,
    int EventId,
    int PartyPowerWeight
);
