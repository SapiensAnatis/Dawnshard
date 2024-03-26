using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

[MemoryPackable]
public partial record RaidEventItem(int Id, int RaidEventId, RaidEventItemType RaidEventItemType);
