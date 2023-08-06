using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public interface IEventReward
{
    int Id { get; }
    int EventItemId { get; }
    int EventItemQuantity { get; }
    EntityTypes RewardEntityType { get; }
    int RewardEntityId { get; }
    int RewardEntityQuantity { get; }
}
