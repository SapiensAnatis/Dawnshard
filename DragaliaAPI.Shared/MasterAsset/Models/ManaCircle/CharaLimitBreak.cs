using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;

public record CharaLimitBreak(
    int Id,
    Materials OrbData1Id1,
    Materials OrbData2Id1,
    Materials OrbData3Id1,
    Materials OrbData4Id1,
    Materials OrbData5Id1,
    int OrbData1Num1,
    int OrbData2Num1,
    int OrbData3Num1,
    int OrbData4Num1,
    int OrbData5Num1,
    int UniqueGrowMaterial1Num1,
    int UniqueGrowMaterial2Num1,
    int GrowMaterialNum1,
    Materials OrbData1Id2,
    Materials OrbData2Id2,
    Materials OrbData3Id2,
    Materials OrbData4Id2,
    Materials OrbData5Id2,
    int OrbData1Num2,
    int OrbData2Num2,
    int OrbData3Num2,
    int OrbData4Num2,
    int OrbData5Num2,
    int UniqueGrowMaterial1Num2,
    int UniqueGrowMaterial2Num2,
    int GrowMaterialNum2,
    Materials OrbData1Id3,
    Materials OrbData2Id3,
    Materials OrbData3Id3,
    Materials OrbData4Id3,
    Materials OrbData5Id3,
    int OrbData1Num3,
    int OrbData2Num3,
    int OrbData3Num3,
    int OrbData4Num3,
    int OrbData5Num3,
    int UniqueGrowMaterial1Num3,
    int UniqueGrowMaterial2Num3,
    int GrowMaterialNum3,
    Materials OrbData1Id4,
    Materials OrbData2Id4,
    Materials OrbData3Id4,
    Materials OrbData4Id4,
    Materials OrbData5Id4,
    int OrbData1Num4,
    int OrbData2Num4,
    int OrbData3Num4,
    int OrbData4Num4,
    int OrbData5Num4,
    int UniqueGrowMaterial1Num4,
    int UniqueGrowMaterial2Num4,
    int GrowMaterialNum4,
    Materials OrbData1Id5,
    Materials OrbData2Id5,
    Materials OrbData3Id5,
    Materials OrbData4Id5,
    Materials OrbData5Id5,
    int OrbData1Num5,
    int OrbData2Num5,
    int OrbData3Num5,
    int OrbData4Num5,
    int OrbData5Num5,
    int UniqueGrowMaterial1Num5,
    int UniqueGrowMaterial2Num5,
    int GrowMaterialNum5
)
{
    public readonly CharaLimitBreakRequirements[] NeededMaterials =
    {
        new(
            new[]
            {
                (OrbData1Id1, OrbData1Num1),
                (OrbData2Id1, OrbData2Num1),
                (OrbData3Id1, OrbData3Num1),
                (OrbData4Id1, OrbData4Num1),
                (OrbData5Id1, OrbData5Num1),
            },
            UniqueGrowMaterial1Num1,
            UniqueGrowMaterial2Num1,
            GrowMaterialNum1
        ),
        new(
            new[]
            {
                (OrbData1Id2, OrbData1Num2),
                (OrbData2Id2, OrbData2Num2),
                (OrbData3Id2, OrbData3Num2),
                (OrbData4Id2, OrbData4Num2),
                (OrbData5Id2, OrbData5Num2),
            },
            UniqueGrowMaterial1Num2,
            UniqueGrowMaterial2Num2,
            GrowMaterialNum2
        ),
        new(
            new[]
            {
                (OrbData1Id3, OrbData1Num3),
                (OrbData2Id3, OrbData2Num3),
                (OrbData3Id3, OrbData3Num3),
                (OrbData4Id3, OrbData4Num3),
                (OrbData5Id3, OrbData5Num3),
            },
            UniqueGrowMaterial1Num3,
            UniqueGrowMaterial2Num3,
            GrowMaterialNum3
        ),
        new(
            new[]
            {
                (OrbData1Id4, OrbData1Num4),
                (OrbData2Id4, OrbData2Num4),
                (OrbData3Id4, OrbData3Num4),
                (OrbData4Id4, OrbData4Num4),
                (OrbData5Id4, OrbData5Num4),
            },
            UniqueGrowMaterial1Num4,
            UniqueGrowMaterial2Num4,
            GrowMaterialNum4
        ),
        new(
            new[]
            {
                (OrbData1Id5, OrbData1Num5),
                (OrbData2Id5, OrbData2Num5),
                (OrbData3Id5, OrbData3Num5),
                (OrbData4Id5, OrbData4Num5),
                (OrbData5Id5, OrbData5Num5),
            },
            UniqueGrowMaterial1Num5,
            UniqueGrowMaterial2Num5,
            GrowMaterialNum5
        ),
    };
}

public record CharaLimitBreakRequirements(
    (Materials Id, int Quantity)[] Orbs,
    int UniqueGrowMaterial1,
    int UniqueGrowMaterial2,
    int GrowMaterial
);
