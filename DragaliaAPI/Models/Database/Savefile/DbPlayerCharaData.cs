using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DragaliaAPI.Models.Dragalia.Enums;
using DragaliaAPI.Models.Enums;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerCharaData")]
public class DbPlayerCharaData : IDbHasAccountId
{
    /// <inheritdoc/>
    [Column("DEVICE_ACCOUNT_ID")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("CHARA_ID")]
    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Charas CharaId { get; set; }

    [Column("RARITY")]
    [Required]
    public byte Rarity { get; set; }

    [Column("EXP")]
    [Required]
    public uint Exp { get; set; }

    [Column("LEVEL")]
    [Required]
    public byte Level { get; set; }

    [Column("ADD_MAX_LVL")]
    [Required]
    public byte AdditionalMaxLevel { get; set; }

    [Column("HP_PLUS_CNT")]
    [Required]
    public byte HpPlusCount { get; set; }

    [Column("ATK_PLUS_CNT")]
    [Required]
    public byte AttackPlusCount { get; set; }

    [Column("LIMIT_BREAK_CNT")]
    [Required]
    public byte LimitBreakCount { get; set; }

    [Column("IS_NEW")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("SKILL_1_LVL")]
    [Required]
    public byte FirstSkillLevel { get; set; }

    [Column("SKILL_2_LVL")]
    [Required]
    public byte SecondSkillLevel { get; set; }

    [Column("ABIL_1_LVL")]
    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Column("ABIL_2_LVL")]
    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Column("ABIL_3_LVL")]
    [Required]
    public byte ThirdAbilityLevel { get; set; }

    [Column("BRST_ATK_LVL")]
    [Required]
    public byte BurstAttackLevel { get; set; }

    [Column("COMBO_BUILDUP_CNT")]
    [Required]
    public uint ComboBuildupCount { get; set; }

    [Column("HP")]
    [Required]
    public ushort Hp { get; set; }

    [Column("ATK")]
    [Required]
    public ushort Attack { get; set; }

    [Column("EX_ABIL_LVL")]
    [Required]
    public byte FirstExAbilityLevel { get; set; }

    [Column("EX_ABIL_2_LVL")]
    [Required]
    public byte SecondExAbilityLevel { get; set; }

    [Column("IS_TEMP")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsTemporary { get; set; }

    [Column("IS_UNLOCK_EDIT_SKILL")]
    [Required]
    public bool IsUnlockEditSkill { get; set; }

    [Column("MANA_NODE_UNLOCK_CNT")]
    [Required]
    public ushort ManaNodeUnlockCount { get; private set; }

    [Column("LIST_VIEW_FLAG")]
    [Required]
    public bool ListViewFlag { get; set; }

    [Column("GET_TIME")]
    [Required]
    public bool GetTime { get; set; }

    [NotMapped]
    public ISet<int> ManaNodesUnlocked
    {
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)ManaNodeUnlockCount);
        set => ManaNodeUnlockCount = (ushort)ManaNodesUtil.SetManaCircleNodesFromSet(value);
    }
}
