using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;

public record ManaPieceType(
    ManaNodeTypes Id,
    EntityTypes EntityType1,
    int EntityId1,
    int EntityQuantity1,
    EntityTypes EntityType2,
    int EntityId2,
    int EntityQuantity2,
    EntityTypes EntityType3,
    int EntityId3,
    int EntityQuantity3
)
{
    public (EntityTypes Type, int Id, int Quantity)[] NeededEntities =
    {
        (EntityType1, EntityId1, EntityQuantity1),
        (EntityType2, EntityId2, EntityQuantity2),
        (EntityType3, EntityId3, EntityQuantity3)
    };
};
