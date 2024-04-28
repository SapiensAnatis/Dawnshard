using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Shop;

public interface IItemSummonService
{
    Task<AtgenUserItemSummon> GetItemSummon();
    IEnumerable<AtgenItemSummonRateList> GetOdds();
    Task<IEnumerable<AtgenBuildEventRewardEntityList>> DoSummon(ShopItemSummonExecRequest request);
}
