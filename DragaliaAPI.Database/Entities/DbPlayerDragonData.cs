using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonData")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerDragonData : IDbHasAccountId, IHasXp
{
    [Column("DragonKeyId")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DragonKeyId { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("DragonId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("Exp")]
    public int Exp { get; set; } = 0;

    [Column("Level")]
    public byte Level { get; set; } = 1;

    [Column("HpPlusCount")]
    public byte HpPlusCount { get; set; } = 0;

    [Column("AttackPlusCount")]
    public byte AttackPlusCount { get; set; } = 0;

    [Column("LimitBreakCount")]
    public byte LimitBreakCount { get; set; } = 0;

    [Column("IsLocked")]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsLock { get; set; } = false;

    [Column("IsNew")]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; } = true;

    [Column("Skill1Level")]
    public byte Skill1Level { get; set; } = 1;

    [Column("Abil1Level")]
    public byte Ability1Level { get; set; } = 1;

    [Column("Abil2Level")]
    public byte Ability2Level { get; set; } = 1;

    [Column("GetTime")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;
}
