using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Enemy;

[MemoryPackable]
public partial record EnemyParam(
    int Id,
    Toughness Tough,
    int DmodeEnemyParamGroupId,
    DmodeEnemyLevelType DmodeEnemyLevelType,
    int Child01Param,
    int Child01Num,
    int Child02Param,
    int Child02Num,
    int Child03Param,
    int Child03Num,
    int Form2nd,
    int WeakA,
    int WeakANum,
    int WeakB,
    int WeakBNum,
    int WeakC,
    int WeakCNum,
    int PartsA,
    int PartsB,
    int PartsC,
    int PartsD,
    int DataId
)
{
    [MemoryPackIgnore]
    public readonly (int Param, int Num)[] Children =
    {
        (Child01Param, Child01Num),
        (Child02Param, Child02Num),
        (Child03Param, Child03Num)
    };

    [MemoryPackIgnore]
    public readonly (int Param, int Num)[] Weaks =
    {
        (WeakA, WeakANum),
        (WeakB, WeakBNum),
        (WeakC, WeakCNum)
    };

    [MemoryPackIgnore]
    public readonly int[] Parts = { PartsA, PartsB, PartsC, PartsD };
};
