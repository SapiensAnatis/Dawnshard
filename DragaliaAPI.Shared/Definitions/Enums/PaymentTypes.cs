namespace DragaliaAPI.Shared.Definitions.Enums;

/// <summary>
/// Payments have weird interactions with <see cref="SummonExecTypes"/> (taken from Summon History):<br/>
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
