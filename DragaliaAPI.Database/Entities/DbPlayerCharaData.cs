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
    public int Exp { get; set; } = 0;

    [Column("Level")]
    public byte Level { get; set; } = 1;

    [Column("AddMaxLevel")]
    public byte AdditionalMaxLevel { get; set; } = 0;

    [Column("HpPlusCount")]
    public byte HpPlusCount { get; set; } = 0;

    [Column("AtkPlusCount")]
    public byte AttackPlusCount { get; set; } = 0;

    [NotMapped]
    public byte LimitBreakCount
    {
        get => (byte)Math.Min(ManaNodeUnlockCount >> 10, ManaNodesUtil.MaxLimitbreakSpiral);
        set => ManaNodeUnlockCount = (ushort)(value << 10);
    }

    [Column("IsNew")]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; } = true;

    [Column("Skill1Lvl")]
    public byte Skill1Level { get; set; } = 1;

    [Column("Skill2Lvl")]
    public byte Skill2Level { get; set; } = 1;

    [Column("Abil1Lvl")]
    public byte Ability1Level { get; set; } = 1;

    [Column("Abil2Lvl")]
    public byte Ability2Level { get; set; } = 1;

    [Column("Abil3Lvl")]
    public byte Ability3Level { get; set; } = 1;

    [Column("BurstAtkLvl")]
    public byte BurstAttackLevel { get; set; } = 1;

    [Column("ComboBuildupCount")]
    public int ComboBuildupCount { get; set; } = 0;

    [Column("HpBase")]
    [Required]
    public ushort HpBase { get; set; }

    [Column("HpNode")]
    [Required]
    public ushort HpNode { get; set; }

    [NotMapped]
    public int Hp => this.HpBase + this.HpNode;

    [Column("AtkBase")]
    [Required]
    public ushort AttackBase { get; set; }

    [Column("AtkNode")]
    [Required]
    public ushort AttackNode { get; set; }

    [NotMapped]
    public int Attack
    {
        get => AttackBase + AttackNode;
    }

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
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)ManaNodeUnlockCount);
        set =>
            ManaNodeUnlockCount = (ushort)
                ManaNodesUtil.SetManaCircleNodesFromSet(value, (ManaNodes)ManaNodeUnlockCount);
    }
}
