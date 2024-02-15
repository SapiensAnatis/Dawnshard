namespace DragaliaAPI.Shared.Definitions.Enums;

public enum CharaPromoteValues
{
    ToRarity4 = 2500,
    ToRarity5 = 25000
}

public enum DupeReturnBaseValues
{
    Rarity4Story = 100,
    Rarity3 = 150,
    Rarity5Story = 300,
    Rarity4 = 2200,
    Rarity5 = 8500
}

public static class DewValueData
{
    /// <summary>
    /// Key: Rarity
    /// Value: Eldwater given for duplicate character summon of that rarity
    /// </summary>
    public static readonly Dictionary<int, int> DupeSummon =
        new()
        {
            { 3, (int)DupeReturnBaseValues.Rarity3 },
            { 4, (int)DupeReturnBaseValues.Rarity4 },
            { 5, (int)DupeReturnBaseValues.Rarity5 }
        };

    /// <summary>
    /// Key: Rarity
    /// Value: Eldwater given for duplicate character summon of that rarity
    /// </summary>
    public static readonly Dictionary<int, int> DupeStorySummon =
        new()
        {
            { 4, (int)DupeReturnBaseValues.Rarity4Story },
            { 5, (int)DupeReturnBaseValues.Rarity5Story }
        };
}

public enum AmuletReturnBaseValues
{
    Rarity3Story = 10,
    Rarity4Story = 100,
    Rarity3 = 150,
    Rarity5Story = 300,
    Rarity4 = 1000,
    Rarity5 = 3000
}
