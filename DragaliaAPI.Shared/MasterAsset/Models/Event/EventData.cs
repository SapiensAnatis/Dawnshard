using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

public record EventData(int Id, EventKindType EventKindType, FortPlants EventFortId);
