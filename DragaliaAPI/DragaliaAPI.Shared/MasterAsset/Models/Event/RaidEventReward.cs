using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record RaidEventReward(
    int Id,
    int RaidEventItemId,
    int RaidEventItemQuantity,
    int IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity
) : IEventReward
{
    [IgnoreMember]
    public int EventItemId => RaidEventItemId;

    [IgnoreMember]
    public int EventItemQuantity => RaidEventItemQuantity;
};
