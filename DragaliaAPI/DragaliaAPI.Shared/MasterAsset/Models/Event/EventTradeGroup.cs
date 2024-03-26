using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record EventTradeGroup(int Id, int EventId);
