using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shop;

namespace DragaliaAPI.Features.Login.Actions;

public class ItemSummonResetAction : IDailyResetAction
{
    private readonly IShopRepository shopRepository;

    public ItemSummonResetAction(IShopRepository shopRepository)
    {
        this.shopRepository = shopRepository;
    }

    public async Task Apply()
    {
        DbPlayerShopInfo info = await this.shopRepository.GetShopInfoAsync();
        info.DailySummonCount = 0;
    }
}
