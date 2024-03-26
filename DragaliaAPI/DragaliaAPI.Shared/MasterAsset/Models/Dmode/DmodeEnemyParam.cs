using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeEnemyParam(
    int Id,
    int DropExp,
    int DropDmodePoint1,
    int DropDmodePoint2,
    int DmodeScore,
    int DmodeEnemyParamGroupId,
    int Level
);
