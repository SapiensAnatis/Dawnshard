using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DragaliaAPI.Models.Data;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerDragonData")]
public class DbPlayerDragonData : IDbHasAccountId
{
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DragonKeyId { get; set; }

    /// <inheritdoc/>
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

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
    public int GetTime { get; set; }
}

public static class DbPlayerDragonDataFactory
{
    public static DbPlayerDragonData Create(string deviceAccountId, int id, int rarity)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = (Dragons)id,
            Exp = 0,
            Level = 1,
            HpPlusCount = 0,
            AttackPlusCount = 0,
            LimitBreakCount = 0,
            IsLocked = false,
            IsNew = true,
            FirstSkillLevel = 1,
            FirstAbilityLevel = 1,
            SecondAbilityLevel = 1,
            GetTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}
