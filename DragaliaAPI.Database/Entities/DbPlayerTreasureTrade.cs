using DragaliaAPI.Features.Shop;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(Id))]
public class DbPlayerTreasureTrade : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("TradeId")]
    public int Id { get; set; }

    [Column("TradeCount")]
    public int Count { get; set; }

    [Column("LastTrade")]
    public DateTimeOffset LastTradeTime { get; set; } = DateTimeOffset.UnixEpoch;
}
