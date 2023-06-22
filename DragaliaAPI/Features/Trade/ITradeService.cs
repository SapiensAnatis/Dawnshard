using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Trade;

public interface ITradeService
{
    IEnumerable<TreasureTradeList> GetCurrentTreasureTradeList();
    Task<IEnumerable<UserTreasureTradeList>> GetUserTreasureTradeList();
    Task DoTreasureTrade(int id, int count, IEnumerable<AtgenNeedUnitList> needUnitList);
}
