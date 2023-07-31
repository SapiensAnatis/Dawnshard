using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record EnemyParam(
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
    int Form2nd
)
{
    public readonly (int Param, int Num)[] Children =
    {
        (Child01Param, Child01Num),
        (Child02Param, Child02Num),
        (Child03Param, Child03Num)
    };
};
