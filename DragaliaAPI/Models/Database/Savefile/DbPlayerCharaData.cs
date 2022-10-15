using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;
using MessagePack;
using DragaliaAPI.Services.Data.Models;

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
    public int Exp { get; set; }

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
    public int ComboBuildupCount { get; set; }

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
    [TypeConverter(typeof(BooleanConverter))]
    public bool ListViewFlag { get; set; }

    [Column("GET_TIME")]
    [Required]
    public int GetTime { get; set; }

    [NotMapped]
    public SortedSet<int> ManaNodesUnlocked
    {
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)ManaNodeUnlockCount);
        set => ManaNodeUnlockCount = (ushort)ManaNodesUtil.SetManaCircleNodesFromSet(value);
    }
}

public static class DbPlayerCharaDataFactory
{
    public static DbPlayerCharaData Create(
        string deviceAccountId,
        DataAdventurer data,
        byte? rarity
    )
    {
        byte validRarity = Math.Clamp(rarity ?? (byte)data.Rarity, (byte)data.Rarity, (byte)5);
        ushort rarityHp;
        ushort rarityAtk;
        switch (validRarity)
        {
            case 3:
                rarityHp = (ushort)data.MinHp3;
                rarityAtk = (ushort)data.MinAtk3;
                break;
            case 4:
                rarityHp = (ushort)data.MinHp4;
                rarityAtk = (ushort)data.MinAtk4;
                break;
            case 5:
            default:
                rarityHp = (ushort)data.MinHp5;
                rarityAtk = (ushort)data.MinAtk5;
                break;
        }

        return new DbPlayerCharaData()
        {
            DeviceAccountId = deviceAccountId,
            CharaId = (Charas)data.IdLong,
            Rarity = validRarity,
            Exp = 0,
            Level = 1,
            AdditionalMaxLevel = 0,
            HpPlusCount = 0,
            AttackPlusCount = 0,
            LimitBreakCount = 0,
            IsNew = true,
            FirstSkillLevel = 1,
            SecondSkillLevel = 1,
            FirstAbilityLevel = 1,
            SecondAbilityLevel = 1,
            ThirdAbilityLevel = 1,
            BurstAttackLevel = 1,
            ComboBuildupCount = 0,
            Hp = rarityHp,
            Attack = rarityAtk,
            FirstExAbilityLevel = 1,
            SecondExAbilityLevel = 1,
            IsTemporary = false,
            IsUnlockEditSkill = false,
            ManaNodesUnlocked = new SortedSet<int>(),
            ListViewFlag = false,
            GetTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}
