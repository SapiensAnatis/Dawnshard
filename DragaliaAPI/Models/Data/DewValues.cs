using System.Reflection;

namespace DragaliaAPI.Models.Data;

public enum CharaPromoteValues
{
    TO_RARITY_4 = 2500,
    TO_RARITY_5 = 25000
}

public enum DupeReturnBaseValues
{
    RARITY_4_STORY = 100,
    RARITY_3 = 150,
    RARITY_5_STORY = 300,
    RARITY_4 = 2200,
    RARITY_5 = 8500
}

public static class DewValueData
{
    /// <summary>
    /// Key: Rarity
    /// Value: Eldwater given for duplicate character summon of that rarity
    /// </summary>
    public static readonly Dictionary<int, int> DupeSummon =
        new() { { 3, 150 }, { 4, 2200 }, { 5, 8500 } };
}

public enum AmuletReturnBaseValues
{
    RARITY_3_STORY = 10,
    RARITY_4_STORY = 100,
    RARITY_3 = 150,
    RARITY_5_STORY = 300,
    RARITY_4 = 1000,
    RARITY_5 = 3000
}
