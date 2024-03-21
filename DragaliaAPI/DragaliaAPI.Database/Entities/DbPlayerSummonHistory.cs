using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerSummonHistory")]
public class DbPlayerSummonHistory : DbPlayerData
{
    [Key]
    public long KeyId { get; set; }

    [Column("BannerId")]
    [Required]
    public int SummonId { get; set; }

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
    public int EntityAttackPlusCount { get; set; }

    [Column("SummonPrizeRank")]
    [Required]
    public SummonPrizeRanks SummonPrizeRank { get; set; }

    /// <summary>
    /// The summon points received.
    /// </summary>
    [Column("SummonPointGet")]
    [Required]
    public int SummonPoint { get; set; }

    [Column("DewPointGet")]
    [Required]
    public int GetDewPointQuantity { get; set; }
}
