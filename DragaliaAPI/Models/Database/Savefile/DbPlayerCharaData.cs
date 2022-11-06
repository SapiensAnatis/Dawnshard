using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Services.Data.Models;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerCharaData")]
public class DbPlayerCharaData : IDbHasAccountId, IHasXp
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

    [NotMapped]
    public byte LimitBreakCount
    {
        get => (byte)Math.Min(ManaNodeUnlockCount >> 10, ManaNodesUtil.MaxLimitbreakSpiral);
        set => ManaNodeUnlockCount = (ushort)(value << 10);
    }

    [Column("IsNew")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Column("Skill1Lvl")]
    [Required]
    public byte FirstSkillLevel { get; set; }

    [Column("Skill2Lvl")]
    [Required]
    public byte SecondSkillLevel { get; set; }

    [Column("Abil1Lvl")]
    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Column("Abil2Lvl")]
    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Column("Abil3Lvl")]
    [Required]
    public byte ThirdAbilityLevel { get; set; }

    [Column("BurstAtkLvl")]
    [Required]
    public byte BurstAttackLevel { get; set; }

    [Column("ComboBuildupCount")]
    [Required]
    public int ComboBuildupCount { get; set; }

    [Column("HpBase")]
    [Required]
    public ushort HpBase { get; set; }

    [Column("HpNode")]
    [Required]
    public ushort HpNode { get; set; }

    [NotMapped]
    public int Hp
    {
        get => HpBase + HpNode;
    }

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
    public byte FirstExAbilityLevel { get; set; }

    [Column("ExAbility2Lvl")]
    [Required]
    public byte SecondExAbilityLevel { get; set; }

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
    public SortedSet<int> ManaNodesUnlocked
    {
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)ManaNodeUnlockCount);
        set =>
            ManaNodeUnlockCount = (ushort)
                ManaNodesUtil.SetManaCircleNodesFromSet(value, (ManaNodes)ManaNodeUnlockCount);
    }
}

public static class DbPlayerCharaDataFactory
{
    public static DbPlayerCharaData Create(
        string deviceAccountId,
        DataAdventurer data,
        byte? rarity = null
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
            HpBase = rarityHp,
            HpNode = 0,
            AttackBase = rarityAtk,
            AttackNode = 0,
            FirstExAbilityLevel = 1,
            SecondExAbilityLevel = 1,
            IsTemporary = false,
            IsUnlockEditSkill = false,
            ManaNodesUnlocked = new SortedSet<int>(),
            ListViewFlag = false,
            GetTime = DateTimeOffset.UtcNow
        };
    }
}
