using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerDragonReliability")]
public class DbPlayerDragonReliability : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DragonId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("Level")]
    [Required]
    public byte ReliabilityLevel { get; set; }

    [Column("TotalExp")]
    [Required]
    public int ReliabilityTotalExp { get; set; }

    [Column("LastContactTime")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset LastContactTime { get; set; }
}

public static class DbPlayerDragonReliabilityFactory
{
    public static DbPlayerDragonReliability Create(string deviceAccountId, Dragons id)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = id,
            ReliabilityLevel = 1,
            ReliabilityTotalExp = 0,
            LastContactTime = DateTimeOffset.UtcNow
        };
    }
}
