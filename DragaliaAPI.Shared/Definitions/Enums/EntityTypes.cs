namespace DragaliaAPI.Shared.Definitions.Enums;

/// <summary>
/// Default Entity_Id is 0
/// </summary>
public enum EntityTypes
{
    Chara = 1,
    Rupies = 4,
    FreeDiamantium = 5,
    PaidDiamantium = 6,
    Dragon = 7,
    Material = 8,
    Dew = 14,
    SkipTicket = 16,
    Mana = 18,

    Wyrmite = 23,

    HustleHammer = 28,
    FafnirMedal = 33,
    SummonSigil = 34,
    Wyrmprint = 39
}

public enum CurrencyTypes
{
    Rupies = EntityTypes.Rupies,
    FreeDiamantium = EntityTypes.FreeDiamantium,
    PaidDiamantium = EntityTypes.PaidDiamantium,
    Dew = EntityTypes.Dew,
    Mana = EntityTypes.Mana,
    Wyrmite = EntityTypes.Wyrmite
}
