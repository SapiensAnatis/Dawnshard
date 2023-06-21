using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Shop;

public interface IShopRepository
{
    IQueryable<DbPlayerShopInfo> ShopInfos { get; }
    Task<DbPlayerShopInfo> GetShopInfoAsync();
    void InitializeShopInfo();
}
