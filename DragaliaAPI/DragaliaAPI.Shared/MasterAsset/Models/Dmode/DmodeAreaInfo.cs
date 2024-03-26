using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeAreaInfo(
    string AreaName,
    int[] EnemyThemes,
    int[] EnemyParams,
    DropObject[] DropObjects
);
