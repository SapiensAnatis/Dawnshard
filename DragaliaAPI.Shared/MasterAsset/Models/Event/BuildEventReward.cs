using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record BuildEventReward(
    int Id,
    int EventItemId,
    int EventItemQuantity,
    int IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity
) : IEventReward;
