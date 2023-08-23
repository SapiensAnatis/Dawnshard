using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(GoodsId))]
public class DbPlayerShopPurchase : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    public int GoodsId { get; set; }
    public PurchaseShopType ShopType { get; set; }
    public int BuyCount { get; set; }
    public DateTimeOffset LastBuyTime { get; set; } = DateTimeOffset.UnixEpoch;
    public DateTimeOffset EffectStartTime { get; set; } = DateTimeOffset.UnixEpoch;
    public DateTimeOffset EffectEndTime { get; set; } = DateTimeOffset.UnixEpoch;
}
