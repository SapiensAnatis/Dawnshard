using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data.Entity;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerDragonData")]
public class DbPlayerDragonData : IDbHasAccountId
{
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DragonKeyId { get; set; }

    /// <inheritdoc/>
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DragonKeyId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("Exp")]
    [Required]
    public uint Exp { get; set; }

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
    public bool IsLocked { get; set; }

    [Column("IsNew")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("Skill1Level")]
    [Required]
    public byte FirstSkillLevel { get; set; }

    [Column("Abil1Level")]
    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Column("Abil2Level")]
    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Column("GetTime")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; }
}

public static class DbPlayerDragonDataFactory
{
    public static DbPlayerDragonData Create(string deviceAccountId, Dragons id)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = id,
            Exp = 0,
            Level = 1,
            HpPlusCount = 0,
            AttackPlusCount = 0,
            LimitBreakCount = 0,
            IsLocked = false,
            IsNew = true,
            FirstSkillLevel = (byte)1,
            FirstAbilityLevel = (byte)1,
            SecondAbilityLevel = (byte)1,
            GetTime = DateTimeOffset.UtcNow
        };
    }
}
