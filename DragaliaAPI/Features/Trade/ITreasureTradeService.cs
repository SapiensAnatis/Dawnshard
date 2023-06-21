using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Trade;

public interface ITreasureTradeService
{
    IEnumerable<TreasureTradeList> GetCurrentTradeList();
    Task<IEnumerable<UserTreasureTradeList>> GetUserTradeList();
    Task DoTreasureTrade(int id, int count, IEnumerable<AtgenNeedUnitList> needUnitList);
}
