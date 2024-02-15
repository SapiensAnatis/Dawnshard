using System.Diagnostics;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Shop;

public enum ShopType
{
    None,
    Special,
    MaterialDaily,
    MaterialWeekly,
    MaterialMonthly,
    Normal
}

public static class ShopTypeExtensions
{
    public static PurchaseShopType ToPurchaseShopType(this ShopType type)
    {
        return type switch
        {
            ShopType.Normal => PurchaseShopType.Normal,
            ShopType.Special => PurchaseShopType.Special,
            ShopType.MaterialDaily => PurchaseShopType.Material,
            ShopType.MaterialWeekly => PurchaseShopType.Material,
            ShopType.MaterialMonthly => PurchaseShopType.Material,
            _ => throw new UnreachableException("Invalid ShopType in repo")
        };
    }
}
