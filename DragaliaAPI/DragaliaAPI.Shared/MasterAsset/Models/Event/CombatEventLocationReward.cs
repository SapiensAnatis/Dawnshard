using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record CombatEventLocationReward(
    int Id,
    int EventId,
    int LocationRewardId,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity
);
