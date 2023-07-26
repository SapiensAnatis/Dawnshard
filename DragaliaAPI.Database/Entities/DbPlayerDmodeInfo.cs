using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId))]
public class DbPlayerDmodeInfo : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("RecoveryCount")]
    public int RecoveryCount { get; set; }

    [Column("RecoveryTime")]
    public DateTimeOffset RecoveryTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("FloorSkipCount")]
    public int FloorSkipCount { get; set; }

    [Column("FloorSkipTime")]
    public DateTimeOffset FloorSkipTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Column("Point1Quantity")]
    public int Point1Quantity { get; set; }

    [Column("Point2Quantity")]
    public int Point2Quantity { get; set; }
}
