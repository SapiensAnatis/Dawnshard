using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record EventItem<T>(int Id, int EventId, T EventItemType)
    where T : struct, Enum;
