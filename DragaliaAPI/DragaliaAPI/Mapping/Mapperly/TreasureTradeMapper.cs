using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Mapping.Mapperly;

public static class TreasureTradeMapper
{
    public static DbPlayerTrade MapToDbPlayerTrade(
        this UserTreasureTradeList tradeList,
        long viewerId
    )
    {
        return new DbPlayerTrade()
        {
            ViewerId = viewerId,
            Id = tradeList.TreasureTradeId,
            Count = tradeList.TradeCount,
            Type = TradeType.Treasure,
            LastTradeTime = tradeList.LastResetTime,
        };
    }
}
