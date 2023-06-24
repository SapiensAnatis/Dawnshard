using System.Diagnostics;

namespace DragaliaAPI.Features.Shop;

public enum MaterialShopType
{
    None,
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
            MaterialShopType.None => throw new UnreachableException("None MaterialShopType"),
            MaterialShopType.Daily => ShopType.MaterialDaily,
            MaterialShopType.Weekly => ShopType.MaterialWeekly,
            MaterialShopType.Monthly => ShopType.MaterialMonthly,
            _ => throw new UnreachableException("Invalid MaterialShopType")
        };
    }
}
