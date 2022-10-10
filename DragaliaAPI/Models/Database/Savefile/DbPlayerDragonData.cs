using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DragaliaAPI.Models.Dragalia.Enums;
using DragaliaAPI.Models.Enums;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerDragonData")]
public class DbPlayerDragonData : IDbHasAccountId
{
    /// <inheritdoc/>
    [Column("DEVICE_ACCOUNT_ID")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DRAGON_KEY_ID")]
    [Required]
    public long DragonKeyId { get; set; }

    [Column("DRAGON_ID")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Column("EXP")]
    [Required]
    public uint Exp { get; set; }

    [Column("LEVEL")]
    [Required]
    public byte Level { get; set; }

    [Column("HP_PLUS_CNT")]
    [Required]
    public byte HpPlusCount { get; set; }

    [Column("ATK_PLUS_CNT")]
    [Required]
    public byte AttackPlusCount { get; set; }

    [Column("LIMIT_BREAK_CNT")]
    [Required]
    public byte LimitBreakCount { get; set; }

    [Column("IS_LOCKED")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsLocked { get; set; }

    [Column("IS_NEW")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("SKILL_1_LVL")]
    [Required]
    public byte FirstSkillLevel { get; set; }

    [Column("ABIL_1_LVL")]
    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Column("ABIL_2_LVL")]
    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Column("GET_TIME")]
    [Required]
    public bool GetTime { get; set; }
}
