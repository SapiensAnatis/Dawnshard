using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record RaidEventItem(int Id, int RaidEventId, RaidEventItemType RaidEventItemType);
