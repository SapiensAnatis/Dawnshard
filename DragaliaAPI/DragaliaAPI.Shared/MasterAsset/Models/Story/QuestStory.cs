using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

public record QuestStory(
    int Id,
    int GroupId,
    PayTargetType PayEntityTargetType,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity
);
