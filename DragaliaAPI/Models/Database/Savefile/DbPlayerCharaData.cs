using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;
using MessagePack;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerCharaData")]
public class DbPlayerCharaData : IDbHasAccountId
{
    /// <inheritdoc/>
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Charas CharaId { get; set; }

    [Required]
    public byte Rarity { get; set; }

    [Required]
    public int Exp { get; set; }

    [Required]
    public byte Level { get; set; }

    [Required]
    public byte AdditionalMaxLevel { get; set; }

    [Required]
    public byte HpPlusCount { get; set; }

    [Required]
    public byte AttackPlusCount { get; set; }

    [Required]
    public byte LimitBreakCount { get; set; }

    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    [Required]
    public byte FirstSkillLevel { get; set; }

    [Required]
    public byte SecondSkillLevel { get; set; }

    [Required]
    public byte FirstAbilityLevel { get; set; }

    [Required]
    public byte SecondAbilityLevel { get; set; }

    [Required]
    public byte ThirdAbilityLevel { get; set; }

    [Required]
    public byte BurstAttackLevel { get; set; }

    [Required]
    public int ComboBuildupCount { get; set; }

    [Required]
    public ushort Hp { get; set; }

    [Required]
    public ushort Attack { get; set; }

    [Required]
    public byte FirstExAbilityLevel { get; set; }

    [Required]
    public byte SecondExAbilityLevel { get; set; }

    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsTemporary { get; set; }

    [Required]
    public bool IsUnlockEditSkill { get; set; }

    [Required]
    public ushort ManaNodeUnlockCount { get; private set; }

    [Required]
    public bool ListViewFlag { get; set; }

    [Required]
    public int GetTime { get; set; }

    [NotMapped]
    public ISet<int> ManaNodesUnlocked
    {
        get => ManaNodesUtil.GetSetFromManaNodes((ManaNodes)ManaNodeUnlockCount);
        set => ManaNodeUnlockCount = (ushort)ManaNodesUtil.SetManaCircleNodesFromSet(value);
    }
}

public static class DbPlayerCharaDataFactory
{
    public static DbPlayerCharaData Create(string deviceAccountId, int id, int rarity)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            CharaId = (Charas)id,
            Rarity = (byte)rarity,
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
            Hp = 100,
            Attack = 100,
            FirstExAbilityLevel = 1,
            SecondExAbilityLevel = 1,
            IsTemporary = false,
            IsUnlockEditSkill = false,
            ManaNodesUnlocked = new HashSet<int>(),
            ListViewFlag = false,
            GetTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}
