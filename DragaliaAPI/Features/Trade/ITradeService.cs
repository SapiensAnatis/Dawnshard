using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Trade;

public interface ITradeService
{
    IEnumerable<TreasureTradeList> GetCurrentTreasureTradeList();
    IEnumerable<AbilityCrestTradeList> GetCurrentAbilityCrestTradeList();

    Task<IEnumerable<UserTreasureTradeList>> GetUserTreasureTradeList();
    Task<IEnumerable<AtgenUserEventTradeList>> GetUserEventTradeList();
    Task<IEnumerable<UserAbilityCrestTradeList>> GetUserAbilityCrestTradeList();

    Task DoTreasureTrade(int id, int count, IEnumerable<AtgenNeedUnitList> needUnitList);
    Task DoAbilityCrestTrade(int id, int count);
}
