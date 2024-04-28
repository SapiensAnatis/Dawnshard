using DragaliaAPI.DTO;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Shop;

public interface IShopService
{
    public Task<IEnumerable<ShopPurchaseList>> DoPurchase(
        ShopType type,
        PaymentTypes paymentType,
        int goodsId,
        int quantity
    );

    public Task<ILookup<PurchaseShopType, ShopPurchaseList>> GetPurchases();
}
