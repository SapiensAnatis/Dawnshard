using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrestTrade(
    int Id,
    AbilityCrests AbilityCrestId,
    int NeedDewPoint,
    int Priority,
    int LabelType
);
