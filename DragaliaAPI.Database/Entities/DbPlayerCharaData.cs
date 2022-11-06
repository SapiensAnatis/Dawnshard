using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerCharaData")]
public class DbPlayerCharaData : IDbHasAccountId
{
    /// <inheritdoc/>
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("CharaId")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Charas CharaId { get; set; }

    [Column("Rarity")]
    [Required]
    public byte Rarity { get; set; }

    [Column("Exp")]
    [Required]
    public int Exp { get; set; }

    [Column("Level")]
    [Required]
    public byte Level { get; set; }

    [Column("AddMaxLevel")]
    [Required]
    public byte AdditionalMaxLevel { get; set; }

    [Column("HpPlusCount")]
    [Required]
    public byte HpPlusCount { get; set; }

    [Column("AtkPlusCount")]
    [Required]
    public byte AttackPlusCount { get; set; }

    [Column("LimitBreakCount")]
    [Required]
    public byte LimitBreakCount { get; set; }

    [Column("IsNew")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("Skill1Lvl")]
    [Required]
    public byte Skill1Level { get; set; }

    [Column("Skill2Lvl")]
    [Required]
    public byte Skill2Level { get; set; }

    [Column("Abil1Lvl")]
    [Required]
    public byte Ability1Level { get; set; }

    [Column("Abil2Lvl")]
    [Required]
    public byte Ability2Level { get; set; }

    [Column("Abil3Lvl")]
    [Required]
    public byte Ability3Level { get; set; }

    [Column("BurstAtkLvl")]
    [Required]
    public byte BurstAttackLevel { get; set; }

    [Column("ComboBuildupCount")]
    [Required]
    public int ComboBuildupCount { get; set; }

    [Column("Hp")]
    [Required]
    public ushort Hp { get; set; }

    [Column("Atk")]
    [Required]
    public ushort Attack { get; set; }

    [Column("ExAbility1Lvl")]
    [Required]
    public byte ExAbilityLevel { get; set; }

    [Column("ExAbility2Lvl")]
    [Required]
    public byte ExAbility2Level { get; set; }

    [Column("IsTemp")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsTemporary { get; set; }

    [Column("IsUnlockEditSkill")]
    [Required]
    public bool IsUnlockEditSkill { get; set; }

    [Column("ManaNodeUnlockCount")]
    [Required]
    public ushort ManaNodeUnlockCount { get; private set; }

    [Column("ListViewFlag")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool ListViewFlag { get; set; }

    [Column("GetTime")]
    [Required]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; }

    [NotMapped]
    public SortedSet<int> ManaCirclePieceIdList
    {
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)this.ManaNodeUnlockCount);
        set => this.ManaNodeUnlockCount = (ushort)ManaNodesUtil.SetManaCircleNodesFromSet(value);
    }
}
