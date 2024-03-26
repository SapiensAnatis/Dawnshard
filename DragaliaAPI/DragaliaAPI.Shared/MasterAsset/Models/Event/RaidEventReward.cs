using DragaliaAPI.Shared.Definitions.Enums;

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
    public int EventItemId => RaidEventItemId;
    public int EventItemQuantity => RaidEventItemQuantity;
};
