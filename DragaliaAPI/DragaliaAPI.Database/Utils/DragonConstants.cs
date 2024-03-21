using System.Collections.Immutable;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Utils;

public static class DragonConstants
{
    public static byte GetMaxLevelFor(int rarity, int limitBreak)
    {
        int baseLevel;
        switch (rarity)
        {
            case 3:
                baseLevel = MinMaxLevel[rarity - 3] + (10 * Math.Min(4, limitBreak));
                break;
            case 4:
                baseLevel =
                    MinMaxLevel[rarity - 3]
                    + (10 * Math.Min(4, limitBreak))
                    + ((limitBreak / 3 > 0 ? 5 : 0) * Math.Clamp(limitBreak - 2, 0, 2));
                break;
            case 5:
                baseLevel = MinMaxLevel[rarity - 3] + (15 * Math.Min(4, limitBreak));
                break;
            default:
                throw new ArgumentException("Invalid rarity");
        }
        return (byte)(baseLevel + (AddMaxLevel * (limitBreak / 5)));
    }

    public const byte MinLevel = 1;
    public static readonly byte[] MinMaxLevel = { 20, 30, 40 };
    public static readonly byte[] MaxLevel = { 60, 80, 100 };
    public const byte AddMaxLevel = 20;
    public const byte MaxAtkEnhance = 50;
    public const byte MaxHpEnhance = 50;

    public const int AugmentResetCost = 20000;

    public static readonly ImmutableArray<int> XpLimits = new[]
    {
        0,
        240,
        540,
        900,
        1320,
        1800,
        2340,
        2940,
        3600,
        4320,
        5100,
        6000,
        7020,
        8160,
        9420,
        10800,
        12300,
        13920,
        15660,
        17520,
        19500,
        21660,
        24000,
        26520,
        29220,
        32100,
        35160,
        38400,
        41820,
        45420,
        49200,
        53220,
        57480,
        61980,
        66720,
        71700,
        76920,
        82380,
        88080,
        94020,
        100200,
        106680,
        113460,
        120540,
        127920,
        135600,
        143580,
        151860,
        160440,
        169320,
        178500,
        188040,
        197940,
        208200,
        218820,
        229800,
        241140,
        252840,
        264900,
        277320,
        290100,
        303330,
        317010,
        331140,
        345720,
        360750,
        376230,
        392160,
        408540,
        425370,
        442650,
        460530,
        479010,
        498090,
        517770,
        538050,
        558930,
        580410,
        602490,
        625170,
        648450,
        672480,
        697260,
        722790,
        749070,
        776100,
        803880,
        832410,
        861690,
        891720,
        922500,
        954180,
        986760,
        1020240,
        1054620,
        1089900,
        1126080,
        1163160,
        1201140,
        1240020,
        1279800,
        1320580,
        1363360,
        1409140,
        1458920,
        1513700,
        1574480,
        1642260,
        1718040,
        1802820,
        1897600,
        2003380,
        2121160,
        2251940,
        2396720,
        2556500,
        2732280,
        2925060,
        3135840,
        3365620
    }.ToImmutableArray();

    public const int MaxRelLevel = 30;

    public static readonly ImmutableArray<DragonGifts> RotatingGifts = new[]
    {
        DragonGifts.GoldenChalice,
        DragonGifts.JuicyMeat,
        DragonGifts.Kaleidoscope,
        DragonGifts.FloralCirclet,
        DragonGifts.CompellingBook,
        DragonGifts.ManaEssence,
        DragonGifts.GoldenChalice
    }.ToImmutableArray();

    public static readonly ImmutableDictionary<DragonGifts, int> FavorVals = new Dictionary<
        DragonGifts,
        int
    >
    {
        { DragonGifts.FreshBread, 100 },
        { DragonGifts.TastyMilk, 300 },
        { DragonGifts.StrawberryTart, 600 },
        { DragonGifts.HeartyStew, 1000 },
        { DragonGifts.JuicyMeat, 1200 },
        { DragonGifts.Kaleidoscope, 1200 },
        { DragonGifts.FloralCirclet, 1200 },
        { DragonGifts.CompellingBook, 1200 },
        { DragonGifts.ManaEssence, 1200 },
        { DragonGifts.GoldenChalice, 2000 },
        { DragonGifts.FourLeafClover, 1000 },
        { DragonGifts.DragonyuleCake, 1000 },
        { DragonGifts.ValentinesCard, 1000 },
        { DragonGifts.PupGrub, 200 }
    }.ToImmutableDictionary();

    public static readonly ImmutableDictionary<DragonGifts, int> BuyGiftPrices = new Dictionary<
        DragonGifts,
        int
    >
    {
        { DragonGifts.FreshBread, 0 },
        { DragonGifts.TastyMilk, 1500 },
        { DragonGifts.StrawberryTart, 4000 },
        { DragonGifts.HeartyStew, 8000 },
        { DragonGifts.JuicyMeat, 12000 },
        { DragonGifts.Kaleidoscope, 12000 },
        { DragonGifts.FloralCirclet, 12000 },
        { DragonGifts.CompellingBook, 12000 },
        { DragonGifts.ManaEssence, 12000 },
        { DragonGifts.GoldenChalice, 15000 }
    }.ToImmutableDictionary();

    public const float FavMulti = 1.5f;

    public static readonly ImmutableArray<int> BondXpLimits = new[]
    {
        0,
        80,
        240,
        420,
        620,
        840,
        1100,
        1400,
        1800,
        2300,
        2900,
        3600,
        4400,
        5300,
        6300,
        7400,
        8600,
        9900,
        11300,
        12800,
        14400,
        16150,
        18050,
        20100,
        22300,
        24700,
        27300,
        30100,
        33100,
        36300
    }.ToImmutableArray();

    public static readonly ImmutableArray<int> BondXpLimitsPuppy = new[]
    {
        0,
        100,
        200,
        300,
        400,
        500,
        600,
        700,
        800,
        900,
        1000,
        1100,
        1200,
        1300,
        1400,
        1500,
        1600,
        1700,
        1800,
        1900,
        2000,
        2100,
        2200,
        2300,
        2400,
        2500,
        2600,
        2700,
        2800,
        2900
    }.ToImmutableArray();

    public static readonly IImmutableSet<Dragons> UnsummonableDragons = new HashSet<Dragons>
    {
        Dragons.BronzeFafnir,
        Dragons.SilverFafnir,
        Dragons.GoldFafnir,
        Dragons.HighMidgardsormr,
        Dragons.HighBrunhilda,
        Dragons.HighMercury,
        Dragons.HighJupiter,
        Dragons.HighZodiark,
        Dragons.MiniMids,
        Dragons.MiniHildy,
        Dragons.MiniMercs,
        Dragons.MiniJupi,
        Dragons.MiniZodi,
        Dragons.Puppy
    }.ToImmutableHashSet();
}
