namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

using MemoryPack;

[MemoryPackable]
public record CombatEventLocation(int Id, int EventId, int LocationRewardId, int ClearQuestId);
