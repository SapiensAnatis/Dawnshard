using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerSummonHistory")]
public class DbPlayerSummonHistory : IDbHasAccountId
{
    [Column("SummonId")]
    [Key]
    public long SummonId { get; set; }

    [Column("DeviceAccountId")]
    [ForeignKey("DbDeviceAccount")]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Column("BannerId")]
    [Required]
    public int BannerId { get; set; }

    [Column("SummonExecType")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public SummonExecTypes SummonExecType { get; set; }

    [Column("SummonDate")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ExecDate { get; set; }

    [Column("PaymentType")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public PaymentTypes PaymentType { get; set; }

    [Column("EntityType")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public EntityTypes EntityType { get; set; }

    [Column("EntityId")]
    [Required]
    public int EntityId { get; set; }

    [Column("Quantity")]
    [Required]
    public int EntityQuantity { get; set; }

    [Column("Level")]
    [Required]
    public byte EntityLevel { get; set; }

    [Column("Rarity")]
    [Required]
    public byte EntityRarity { get; set; }

    [Column("LimitBreakCount")]
    [Required]
    public byte EntityLimitBreakCount { get; set; }

    [Column("HpPlusCount")]
    [Required]
    public int EntityHpPlusCount { get; set; }

    [Column("AtkPlusCount")]
    [Required]
    public int EntityAtkPlusCount { get; set; }

    [Column("SummonPrizeRank")]
    [Required]
    public SummonPrizeRanks SummonPrizeRank { get; set; }

    [Column("SummonPointGet")]
    [Required]
    public int SummonPointGet { get; set; }

    [Column("DewPointGet")]
    [Required]
    public int DewPointGet { get; set; }
}
