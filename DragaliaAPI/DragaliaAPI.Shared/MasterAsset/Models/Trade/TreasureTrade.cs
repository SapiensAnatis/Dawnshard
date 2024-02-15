using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Trade;

public record TreasureTrade(
    int Id,
    int TradeGroupId,
    int Limit,
    int Priority,
    int TabGroupId,
    DateTimeOffset CommenceDate,
    DateTimeOffset CompleteDate,
    int IsLockView,
    int ResetType,
    EntityTypes NeedEntityType1,
    int NeedEntityId1,
    int NeedEntityQuantity1,
    int NeedLimitBreakCount1,
    EntityTypes NeedEntityType2,
    int NeedEntityId2,
    int NeedEntityQuantity2,
    int NeedLimitBreakCount2,
    EntityTypes NeedEntityType3,
    int NeedEntityId3,
    int NeedEntityQuantity3,
    int NeedLimitBreakCount3,
    EntityTypes NeedEntityType4,
    int NeedEntityId4,
    int NeedEntityQuantity4,
    int NeedLimitBreakCount4,
    EntityTypes NeedEntityType5,
    int NeedEntityId5,
    int NeedEntityQuantity5,
    int NeedLimitBreakCount5,
    EntityTypes DestinationEntityType,
    int DestinationEntityId,
    int DestinationEntityQuantity,
    int DestinationLimitBreakCount
)
{
    public (EntityTypes Type, int Id, int Quantity, int LimitBreakCount)[] NeedEntities =>
        new[]
        {
            (NeedEntityType1, NeedEntityId1, NeedEntityQuantity1, NeedLimitBreakCount1),
            (NeedEntityType2, NeedEntityId2, NeedEntityQuantity2, NeedLimitBreakCount2),
            (NeedEntityType3, NeedEntityId3, NeedEntityQuantity3, NeedLimitBreakCount3),
            (NeedEntityType4, NeedEntityId4, NeedEntityQuantity4, NeedLimitBreakCount4),
            (NeedEntityType5, NeedEntityId5, NeedEntityQuantity5, NeedLimitBreakCount5)
        };
};
