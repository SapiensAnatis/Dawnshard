using System.Collections.Immutable;

namespace DragaliaAPI.Database.Utils;

public static class CharaConstants
{
    public static byte GetMaxLevelFor(int rarity)
    {
        return rarity is < 3 or > 5
            ? throw new ArgumentException("Invalid Rarity")
            : (byte)(MinMaxLevel + ((rarity - 3) * 10));
    }

    public const byte MinLevel = 1;
    public const byte MinMaxLevel = 60;
    public const byte MaxLevel = 80;
    public const byte AddMaxLevel = 20;
    public const byte MaxAtkEnhance = 100;
    public const byte MaxHpEnhance = 100;

    public const int AugmentResetCost = 20000;

    public static readonly ImmutableList<int> XpLimits = new List<int>
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
