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
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    public long DragonKeyId { get; set; }

    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Required]
    public uint Exp { get; set; }

    [Required]
    public byte Level { get; set; }

    [Required]
    public byte HpPlusCount { get; set; }

    [Required]
    public byte AttackPlusCount { get; set; }

    [Required]
    public byte LimitBreakCount { get; set; }

    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsLocked { get; set; }

    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Required]
    public byte FirstSkillLevel { get; set; }

    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Required]
    public bool GetTime { get; set; }
}
