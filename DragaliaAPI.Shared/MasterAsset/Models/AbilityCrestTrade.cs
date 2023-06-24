using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestTrade(
    int Id,
    AbilityCrests AbilityCrestId,
    int NeedDewPoint,
    int Priority,
    DateTimeOffset CompleteDate,
    DateTimeOffset PickupViewStartDate,
    DateTimeOffset PickupViewEndDate
);
