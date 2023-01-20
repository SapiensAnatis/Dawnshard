using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonReliability")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerDragonReliability : IDbHasAccountId, IHasXp
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("DragonId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("Level")]
    [Required]
    public byte Level { get; set; }

    [Column("TotalExp")]
    [Required]
    public int Exp { get; set; }

    [Column("LastContactTime")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset LastContactTime { get; set; }

    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; }
}

public static class DbPlayerDragonReliabilityFactory
{
    public static DbPlayerDragonReliability Create(string deviceAccountId, Dragons id)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = id,
            Level = 1,
            Exp = 0,
            GetTime = DateTimeOffset.UtcNow,
            LastContactTime = DateTimeOffset.UtcNow
        };
    }
}
