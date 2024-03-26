using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record BuildEventReward(
    int Id,
    int EventItemId,
    int EventItemQuantity,
    int IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity
) : IEventReward;
