using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
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
        byte defaultRelLevel = (byte)MasterAsset.DragonData.Get(id).DefaultReliabilityLevel;
        defaultRelLevel = defaultRelLevel == default ? (byte)1 : defaultRelLevel;
        DbPlayerDragonReliability newReliability = new DbPlayerDragonReliability()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = id,
            Level = defaultRelLevel,
            Exp = DragonConstants.bondXpLimits[defaultRelLevel - 1],
            GetTime = DateTimeOffset.UtcNow,
            LastContactTime = DateTimeOffset.UtcNow
        };
        return newReliability;
    }
}
