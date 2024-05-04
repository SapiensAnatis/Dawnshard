namespace DragaliaAPI.Shared.Definitions.Enums;

/// <summary>
/// Payments have weird interactions with <see cref="Summon.SummonExecTypes"/> (taken from Summon History):<br/>
/// <see cref="Diamantium"/> and <see cref="Wyrmite"/> work with Single, Tenfold and DailyDeal<br/>
/// <see cref="Ticket"/> and <see cref="FreeDailyExecDependant"/> only for Single and Tenfold<br/>
/// <see cref="FreeDailyTenfold"/> is <b>ALWAYS</b> Daily Free Tenfold
/// </summary>
public enum PaymentTypes
{
    None,
    Money,
    Diamantium,
    Wyrmite,
    DiamantiumOrWyrmite,
    Coin,
    ManaPoint,
    DewPoint,
    Ticket,
    FreeDailyExecDependant,
    FreeDailyTenfold,
    HalidomHustleHammer,
    ItemSummonCampaign,
    TutorialTicketSummon,
    SummonCampaignOneHundred,
    Other = 99
}

public static class PaymentTypesExtensions
{
    public static EntityTypes ToEntityType(this PaymentTypes type)
    {
        return type switch
        {
            PaymentTypes.Wyrmite => EntityTypes.Wyrmite,
            PaymentTypes.Coin => EntityTypes.Rupies,
            PaymentTypes.ManaPoint => EntityTypes.Mana,
            PaymentTypes.DewPoint => EntityTypes.Dew,
            PaymentTypes.Ticket => EntityTypes.SummonTicket,
            PaymentTypes.HalidomHustleHammer => EntityTypes.HustleHammer,
            _ => EntityTypes.None
        };
    }
}
