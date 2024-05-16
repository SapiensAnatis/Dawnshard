using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Trade;

public interface ITradeService
{
    IEnumerable<TreasureTradeList> GetCurrentTreasureTradeList();
    IEnumerable<AbilityCrestTradeList> GetCurrentAbilityCrestTradeList();
    IEnumerable<EventTradeList> GetEventTradeList(int tradeGroupId);

    Task<IEnumerable<UserTreasureTradeList>> GetUserTreasureTradeList();
    Task<IEnumerable<AtgenUserEventTradeList>> GetUserEventTradeList();
    Task<IEnumerable<UserAbilityCrestTradeList>> GetUserAbilityCrestTradeList();

    Task DoTrade(
        TradeType type,
        int id,
        int count,
        IEnumerable<AtgenNeedUnitList>? needUnitList = null
    );
    Task DoAbilityCrestTrade(int id, int count);
    EntityResult GetEntityResult();
}
