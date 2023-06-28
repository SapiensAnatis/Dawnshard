using DragaliaAPI.Features.Shop;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V3Update : ISavefileUpdate
{
    private readonly IShopRepository shopRepository;

    public V3Update(IShopRepository shopRepository)
    {
        this.shopRepository = shopRepository;
    }

    public int SavefileVersion => 3;

    public async Task Apply()
    {
        if (!await this.shopRepository.ShopInfos.AnyAsync())
        {
            this.shopRepository.InitializeShopInfo();
        }
    }
}
