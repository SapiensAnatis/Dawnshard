using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

using MemoryPack;

[MemoryPackable]
public record RaidEventItem(int Id, int RaidEventId, RaidEventItemType RaidEventItemType);
