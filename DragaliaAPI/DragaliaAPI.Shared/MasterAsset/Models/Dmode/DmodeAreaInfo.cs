using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeAreaInfo(
    string AreaName,
    int[] EnemyThemes,
    int[] EnemyParams,
    DropObject[] DropObjects
);
