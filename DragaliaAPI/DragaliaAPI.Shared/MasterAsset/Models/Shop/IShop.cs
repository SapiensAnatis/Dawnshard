using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Shop;

public interface IShop
{
    public int Id { get; }
    public int Limit { get; }
    public PaymentTypes PaymentType { get; }
    public int NeedCost { get; }
    public EntityTypes DestinationEntityType1 { get; }
    public int DestinationEntityId1 { get; }
    public int DestinationEntityQuantity1 { get; }
    public int DestinationLimitBreakCount1 { get; }
    public EntityTypes DestinationEntityType2 { get; }
    public int DestinationEntityId2 { get; }
    public int DestinationEntityQuantity2 { get; }
    public int DestinationLimitBreakCount2 { get; }
    public EntityTypes DestinationEntityType3 { get; }
    public int DestinationEntityId3 { get; }
    public int DestinationEntityQuantity3 { get; }
    public int DestinationLimitBreakCount3 { get; }
    public EntityTypes DestinationEntityType4 { get; }
    public int DestinationEntityId4 { get; }
    public int DestinationEntityQuantity4 { get; }
    public int DestinationLimitBreakCount4 { get; }
    public EntityTypes DestinationEntityType5 { get; }
    public int DestinationEntityId5 { get; }
    public int DestinationEntityQuantity5 { get; }
    public int DestinationLimitBreakCount5 { get; }
    public EntityTypes DestinationEntityType6 { get; }
    public int DestinationEntityId6 { get; }
    public int DestinationEntityQuantity6 { get; }
    public int DestinationLimitBreakCount6 { get; }
    public EntityTypes DestinationEntityType7 { get; }
    public int DestinationEntityId7 { get; }
    public int DestinationEntityQuantity7 { get; }
    public int DestinationLimitBreakCount7 { get; }
    public EntityTypes DestinationEntityType8 { get; }
    public int DestinationEntityId8 { get; }
    public int DestinationEntityQuantity8 { get; }
    public int DestinationLimitBreakCount8 { get; }

    public (EntityTypes Type, int Id, int Quantity, int LimitBreakCount)[] DestinationEntities =>
        new[]
        {
            (
                DestinationEntityType1,
                DestinationEntityId1,
                DestinationEntityQuantity1,
                DestinationLimitBreakCount1
            ),
            (
                DestinationEntityType2,
                DestinationEntityId2,
                DestinationEntityQuantity2,
                DestinationLimitBreakCount2
            ),
            (
                DestinationEntityType3,
                DestinationEntityId3,
                DestinationEntityQuantity3,
                DestinationLimitBreakCount3
            ),
            (
                DestinationEntityType4,
                DestinationEntityId4,
                DestinationEntityQuantity4,
                DestinationLimitBreakCount4
            ),
            (
                DestinationEntityType5,
                DestinationEntityId5,
                DestinationEntityQuantity5,
                DestinationLimitBreakCount5
            ),
            (
                DestinationEntityType6,
                DestinationEntityId6,
                DestinationEntityQuantity6,
                DestinationLimitBreakCount6
            ),
            (
                DestinationEntityType7,
                DestinationEntityId7,
                DestinationEntityQuantity7,
                DestinationLimitBreakCount7
            ),
            (
                DestinationEntityType8,
                DestinationEntityId8,
                DestinationEntityQuantity8,
                DestinationLimitBreakCount8
            )
        };
}
