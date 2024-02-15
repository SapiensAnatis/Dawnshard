using System.Diagnostics;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Shop;

namespace DragaliaAPI.Features.Shop;

public static class Shop
{
    public static IShop From(ShopType type, int goodsId)
    {
        return type switch
        {
            ShopType.None => throw new UnreachableException("None ShopType"),
            ShopType.Normal => MasterAsset.NormalShop[goodsId],
            ShopType.Special => MasterAsset.SpecialShop[goodsId],
            ShopType.MaterialDaily => MasterAsset.MaterialShopDaily[goodsId],
            ShopType.MaterialWeekly => MasterAsset.MaterialShopWeekly[goodsId],
            ShopType.MaterialMonthly => MasterAsset.MaterialShopMonthly[goodsId],
            _ => throw new UnreachableException("Invalid ShopType")
        };
    }
}
