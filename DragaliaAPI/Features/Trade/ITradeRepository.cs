using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Trade;

public interface ITradeRepository
{
    IQueryable<DbPlayerTreasureTrade> TreasureTrades { get; }

    Task<bool> AddTrade(int id, int count, DateTimeOffset time);
}
