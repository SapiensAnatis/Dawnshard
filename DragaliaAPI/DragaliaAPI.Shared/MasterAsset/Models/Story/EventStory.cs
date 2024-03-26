using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Story;

[MemoryPackable]
public partial record EventStory(
    int Id,
    int EventId,
    int BaseId,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity
);
