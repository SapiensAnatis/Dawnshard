using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[Index(nameof(DeviceAccountId), nameof(Type))]
[PrimaryKey(nameof(DeviceAccountId), nameof(Id))]
public class DbPlayerTrade : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("TradeType")]
    public required TradeType Type { get; set; }

    [Column("TradeId")]
    public required int Id { get; set; }

    [Column("TradeCount")]
    public int Count { get; set; }

    [Column("LastTrade")]
    public DateTimeOffset LastTradeTime { get; set; } = DateTimeOffset.UnixEpoch;
}
