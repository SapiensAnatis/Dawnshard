using System.Diagnostics;

namespace DragaliaAPI.Shared.Definitions.Enums;

/// <summary>
/// Default Entity_Id is 0
/// </summary>
public enum EntityTypes
{
    None,
    Chara,
    Item,
    Weapon,
    Rupies,
    FreeDiamantium,
    PaidDiamantium,
    Dragon,
    Material,
    FortPlant,
    Title,
    Stamp,
    Amulet,
    DungeonItem,
    Dew,
    DragonGift,
    SkipTicket,
    SummonTicket,
    Mana,
    ExchangeTicket,
    RaidEventItem,
    MazeEventItem,
    BuildEventItem,
    Wyrmite,
    CollectEventItem,
    Clb01EventItem,
    AstralItem,
    GuildEmblem,
    HustleHammer,
    ExRushEventItem,
    SimpleEventItem,
    LotteryTicket,
    ExHunterEventItem,
    FafnirMedal,
    CombatEventItem, // SummonSigil
    SummonPoint,
    BattleRoyalEventItem,
    WeaponSkin,
    WeaponBody,
    Wyrmprint,
    EarnEventItem,
    Talisman,
    DmodePoint,
    DmodeDungeonItem
}

public static class EntityTypesExtensions
{
    public static PaymentTypes ToPaymentType(this EntityTypes entityTypes)
    {
        return entityTypes switch
        {
            EntityTypes.Rupies => PaymentTypes.Coin,
            EntityTypes.Dew => PaymentTypes.DewPoint,
            EntityTypes.Mana => PaymentTypes.ManaPoint,
            EntityTypes.Wyrmite => PaymentTypes.Wyrmite,
            _ => throw new UnreachableException("Invalid EntityType for PaymentType conversion")
        };
    }
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
