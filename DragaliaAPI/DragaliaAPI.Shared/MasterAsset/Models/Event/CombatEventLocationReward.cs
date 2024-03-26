using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record CombatEventLocationReward(
    int Id,
    int EventId,
    int LocationRewardId,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity
);
