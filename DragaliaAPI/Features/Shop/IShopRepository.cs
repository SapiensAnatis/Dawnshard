using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Shop;

public interface IShopRepository
{
    IQueryable<DbPlayerShopInfo> ShopInfos { get; }
    IQueryable<DbPlayerShopPurchase> Purchases { get; }
    Task<DbPlayerShopInfo> GetShopInfoAsync();

    void InitializeShopInfo();
    Task<int> GetDailySummonCountAsync();

    public Task ClearExpiredShopPurchases();
    Task<bool> AddShopPurchase(
        ShopType type,
        int goodsId,
        int quantity,
        DateTimeOffset buyTime,
        DateTimeOffset effectStart,
        DateTimeOffset effectEnd
    );
}
