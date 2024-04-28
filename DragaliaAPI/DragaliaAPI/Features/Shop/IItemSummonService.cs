using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Shop;

public interface IItemSummonService
{
    Task<AtgenUserItemSummon> GetItemSummon();
    IEnumerable<AtgenItemSummonRateList> GetOdds();
    Task<IEnumerable<AtgenBuildEventRewardEntityList>> DoSummon(ShopItemSummonExecRequest request);
}
