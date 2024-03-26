using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

[MemoryPackable]
public partial record QuestStory(
    int Id,
    int GroupId,
    PayTargetType PayEntityTargetType,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity
);
