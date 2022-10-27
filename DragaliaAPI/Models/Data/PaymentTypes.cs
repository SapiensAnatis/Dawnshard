namespace DragaliaAPI.Models.Data;

/// <summary>
/// Payments have weird interactions with <see cref="SummonExecTypes"/> (taken from Summon History):<br/>
/// <see cref="Diamanitum"/> and <see cref="Wyrmite"/> work with Single, Tenfold and DailyDeal<br/>
/// <see cref="Ticket"/> and <see cref="FreeDailyExecDependant"/> only for Single and Tenfold<br/>
/// <see cref="FreeDailyTenfold"/> is <b>ALWAYS</b> Daily Free Tenfold
/// </summary>
public enum PaymentTypes
{
    Diamantium = 2,
    Wyrmite = 3,
    Ticket = 8,

    FreeDailyExecDependant = 9,
    FreeDailyTenfold = 10
}
