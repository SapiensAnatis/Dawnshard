using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record EventPassive(int Id, int EventId, int MaxGrowValue, int ViewRarity);
