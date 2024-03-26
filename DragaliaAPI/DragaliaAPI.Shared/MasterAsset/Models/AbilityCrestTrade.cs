using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record AbilityCrestTrade(
    int Id,
    AbilityCrests AbilityCrestId,
    int NeedDewPoint,
    int Priority,
    DateTimeOffset CompleteDate,
    DateTimeOffset PickupViewStartDate,
    DateTimeOffset PickupViewEndDate
);
