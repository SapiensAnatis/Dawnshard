using System.Diagnostics;

namespace DragaliaAPI.Features.Shop;

public enum MaterialShopType
{
    Daily,
    Weekly,
    Monthly
}

public static class MaterialShopTypeExtensions
{
    public static ShopType ToShopType(this MaterialShopType type)
    {
        return type switch
        {
            MaterialShopType.Daily => ShopType.MaterialDaily,
            MaterialShopType.Weekly => ShopType.MaterialWeekly,
            MaterialShopType.Monthly => ShopType.MaterialMonthly,
            _ => throw new UnreachableException("Invalid MaterialShopType")
        };
    }
}
