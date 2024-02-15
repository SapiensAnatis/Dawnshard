namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record EventItem<T>(int Id, int EventId, T EventItemType)
    where T : struct, Enum;
