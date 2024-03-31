using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;

public record ManaPieceMaterial(
    int Id,
    int ElementId,
    ManaNodeTypes ManaPieceType,
    int Step,
    Materials MaterialId1,
    int MaterialQuantity1,
    Materials MaterialId2,
    int MaterialQuantity2,
    Materials MaterialId3,
    int MaterialQuantity3,
    int DewPoint
)
{
    [IgnoreMember]
    public (Materials Id, int Quantity)[] NeededMaterials =
    {
        (MaterialId1, MaterialQuantity1),
        (MaterialId2, MaterialQuantity2),
        (MaterialId3, MaterialQuantity3),
    };
};
