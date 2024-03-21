using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId))]
public class DbPlayerDmodeInfo : DbPlayerData
{
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
