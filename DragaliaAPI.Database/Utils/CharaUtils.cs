using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Database.Utils;

public static class CharaUtils
{
    public static byte GetMaxLevelFor(int rarity)
    {
        return rarity < 3 || rarity > 5
            ? throw new ArgumentException("Invalid Rarity")
            : (byte)(MinMaxLevel + ((rarity - 3) * 10));
    }

    public static int CalculateBaseHp(CharaData adventurer, int level, int rarity)
    {
        double hpStep;
        int hpBase;
        int lvlBase;

        if (level > MaxLevel)
        {
            hpStep = (adventurer.AddMaxHp1 - adventurer.MaxHp) / (double)AddMaxLevel;
            hpBase = adventurer.MaxHp;
            lvlBase = MaxLevel;
        }
        else
        {
            hpStep = (adventurer.MaxHp - adventurer.MinHp5) / (double)(MaxLevel - MinLevel);
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

    public static int CalculateBaseAttack(CharaData adventurer, int level, int rarity)
    {
        double atkStep;
        int atkBase;
        int lvlBase;

        if (level > MaxLevel)
        {
            atkStep = (adventurer.AddMaxAtk1 - adventurer.MaxAtk) / (double)AddMaxLevel;
            atkBase = adventurer.MaxAtk;
            lvlBase = MaxLevel;
        }
        else
        {
            atkStep = (adventurer.MaxAtk - adventurer.MinAtk5) / (double)(MaxLevel - MinLevel);
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

    private const byte MinLevel = 1;
    private const byte MinMaxLevel = 60;
    private const byte MaxLevel = 80;
    private const byte AddMaxLevel = 20;
}
