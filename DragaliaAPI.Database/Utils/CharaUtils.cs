using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Database.Utils;

public static class CharaUtils
{
    public static byte GetMaxLevelFor(int rarity)
    {
        return rarity < 3 || rarity > 5
            ? throw new ArgumentException("Invalid Rarity")
            : (byte)(MinMaxLevel + ((rarity - 3) * 10));
    }

    public static int CalcMight(
        DbPlayerCharaData dbCharData,
        DataAdventurer charData,
        bool addSharedSkillMight = false,
        bool isLeader = false
    )
    {
        return dbCharData.Hp
            + dbCharData.HpPlusCount
            + dbCharData.Attack
            + dbCharData.AttackPlusCount
            + fsMights[dbCharData.BurstAttackLevel]
            + skillMights[dbCharData.Skill1Level]
            + skillMights[dbCharData.Skill1Level]
            // a1 Might
            + 0
            // a2 Might
            + 0
            // a3 Might
            + 0
            // Ex Might
            + 0
            //sharedSkillMight
            + (addSharedSkillMight ? 0 : 0)
            + (isLeader ? 200 : 0);
    }

    public static int CalculateBaseHp(DataAdventurer adventurer, int level, int rarity)
    {
        double hpStep;
        int hpBase;
        int lvlBase;

        if (level > MaxLevel)
        {
            hpStep = (adventurer.AddMaxHp1 - adventurer.MaxHp) / AddMaxLevel;
            hpBase = adventurer.MaxHp;
            lvlBase = MaxLevel;
        }
        else
        {
            hpStep = (adventurer.MaxHp - adventurer.MinHp5) / (MaxLevel - MinLevel);
            hpBase = rarity switch
            {
                3 => adventurer.MinHp3,
                4 => adventurer.MinHp4,
                5 => adventurer.MinHp5,
                _ => throw new ArgumentException("Invalid rarity!")
            };
            lvlBase = MinLevel;
        }

        return (int)Math.Ceiling((hpStep * (level - lvlBase)) + hpBase);
    }

    public static int CalculateBaseAttack(DataAdventurer adventurer, int level, int rarity)
    {
        double atkStep;
        int atkBase;
        int lvlBase;

        if (level > MaxLevel)
        {
            atkStep = (adventurer.AddMaxAtk1 - adventurer.MaxAtk) / AddMaxLevel;
            atkBase = adventurer.MaxAtk;
            lvlBase = MaxLevel;
        }
        else
        {
            atkStep = (adventurer.MaxAtk - adventurer.MinAtk5) / (MaxLevel - MinLevel);
            atkBase = rarity switch
            {
                3 => adventurer.MinAtk3,
                4 => adventurer.MinAtk4,
                5 => adventurer.MinAtk5,
                _ => throw new ArgumentException("Invalid rarity!")
            };
            lvlBase = MinLevel;
        }

        return (int)Math.Ceiling((atkStep * (level - lvlBase)) + atkBase);
    }

    public const byte MinLevel = 1;
    public const byte MinMaxLevel = 60;
    public const byte MaxLevel = 80;
    public const byte AddMaxLevel = 20;
    public const byte MaxAtkEnhance = 100;
    public const byte MaxHpEnhance = 100;

    public readonly static int[] skillMights = new int[] { 0, 100, 200, 300, 400 };
    public readonly static int[] fsMights = new int[] { 0, 60, 120 };

    public static readonly ImmutableList<int> XpLimits = new List<int>()
    {
        0,
        30,
        80,
        150,
        240,
        350,
        500,
        690,
        920,
        1190,
        1500,
        1880,
        2330,
        2850,
        3440,
        4100,
        4880,
        5780,
        6800,
        7940,
        9200,
        10710,
        12470,
        14480,
        16740,
        19250,
        22260,
        25770,
        29780,
        34290,
        39300,
        45060,
        51570,
        58830,
        66840,
        75600,
        85110,
        95370,
        106380,
        118140,
        130650,
        143910,
        157920,
        172680,
        188190,
        204450,
        221460,
        239220,
        257730,
        276990,
        297000,
        317760,
        339270,
        361530,
        384540,
        408300,
        432810,
        458070,
        484080,
        510840,
        538350,
        570950,
        603750,
        636750,
        669950,
        703350,
        736950,
        770750,
        804750,
        838950,
        873350,
        907950,
        942750,
        977750,
        1012950,
        1048350,
        1083950,
        1119750,
        1155750,
        1191950,
        1391950,
        1606950,
        1836950,
        2081950,
        2341950,
        2616950,
        2906950,
        3211950,
        3531950,
        3866950,
        4216950,
        4596950,
        5006950,
        5446950,
        5916950,
        6416950,
        6961950,
        7551950,
        8186950,
        8866950
    }.ToImmutableList();
}
