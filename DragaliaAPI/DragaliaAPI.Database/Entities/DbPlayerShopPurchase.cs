using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(GoodsId))]
public class DbPlayerShopPurchase : DbPlayerData
{
    public int GoodsId { get; set; }
    public PurchaseShopType ShopType { get; set; }
    public int BuyCount { get; set; }
    public DateTimeOffset LastBuyTime { get; set; } = DateTimeOffset.UnixEpoch;
    public DateTimeOffset EffectStartTime { get; set; } = DateTimeOffset.UnixEpoch;
    public DateTimeOffset EffectEndTime { get; set; } = DateTimeOffset.UnixEpoch;
}
