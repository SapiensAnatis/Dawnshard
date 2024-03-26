using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record CombatEventLocation(
    int Id,
    int EventId,
    int LocationRewardId,
    int ClearQuestId
);
