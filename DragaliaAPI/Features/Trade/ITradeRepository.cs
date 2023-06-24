using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Trade;

public interface ITradeRepository
{
    IQueryable<DbPlayerTrade> Trades { get; }

    Task<ILookup<TradeType, DbPlayerTrade>> GetAllTradesAsync();
    Task<IEnumerable<DbPlayerTrade>> GetTradesByTypeAsync(TradeType type);

    Task<bool> AddTrade(TradeType type, int id, int count, DateTimeOffset? time = null);
}
