namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

using MemoryPack;

[MemoryPackable]
public record EventItem<T>(int Id, int EventId, T EventItemType)
    where T : struct, Enum;
