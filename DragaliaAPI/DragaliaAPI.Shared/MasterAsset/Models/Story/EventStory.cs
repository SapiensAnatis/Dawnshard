using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

public record EventStory(
    int Id,
    int EventId,
    int BaseId,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity
);
