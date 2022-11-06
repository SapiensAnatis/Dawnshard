using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonData")]
public class DbPlayerDragonData : IDbHasAccountId, IHasXp
{
    [Column("DragonKeyId")]
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DragonKeyId { get; set; }

    /// <inheritdoc/>
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DragonId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("Exp")]
    [Required]
    public int Exp { get; set; }

    [Column("Level")]
    [Required]
    public byte Level { get; set; }

    [Column("HpPlusCount")]
    [Required]
    public byte HpPlusCount { get; set; }

    [Column("AttackPlusCount")]
    [Required]
    public byte AttackPlusCount { get; set; }

    [Column("LimitBreakCount")]
    [Required]
    public byte LimitBreakCount { get; set; }

    [Column("IsLocked")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsLock { get; set; }

    [Column("IsNew")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("Skill1Level")]
    [Required]
    public byte Skill1Level { get; set; }

    [Column("Abil1Level")]
    [Required]
    public byte Ability1Level { get; set; }

    [Column("Abil2Level")]
    [Required]
    public byte Ability2Level { get; set; }

    [Column("GetTime")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; }
}
