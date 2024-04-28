using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

public record EventStory(
    int Id,
    int EventId,
    int BaseId,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity
);
