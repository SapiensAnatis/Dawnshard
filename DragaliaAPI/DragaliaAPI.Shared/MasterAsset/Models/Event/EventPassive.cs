namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

using MemoryPack;

[MemoryPackable]
public record EventPassive(int Id, int EventId, int MaxGrowValue, int ViewRarity);
