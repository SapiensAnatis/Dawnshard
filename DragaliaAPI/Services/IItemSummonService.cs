using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IItemSummonService
{
    public IEnumerable<AtgenItemSummonRateList> GetOdds();
    public Task<List<AtgenBuildEventRewardEntityList>> DoSummon(ShopItemSummonExecRequest request);
}
