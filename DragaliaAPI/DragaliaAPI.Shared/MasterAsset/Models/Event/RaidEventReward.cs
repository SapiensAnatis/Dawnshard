using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record RaidEventReward(
    int Id,
    int RaidEventItemId,
    int RaidEventItemQuantity,
    int IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity
) : IEventReward
{
    public int EventItemId => RaidEventItemId;
    public int EventItemQuantity => RaidEventItemQuantity;
};
