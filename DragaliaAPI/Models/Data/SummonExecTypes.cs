namespace DragaliaAPI.Models.Data;

/// <summary>
/// SummonExecTypes have weird interactions with <see cref="PaymentTypes"/> (taken from Summon History):<br/>
/// <see cref="PaymentTypes.Diamanitum"/> and <see cref="PaymentTypes.Wyrmite"/> work with Single, Tenfold and DailyDeal<br/>
/// <see cref="PaymentTypes.Ticket"/> and <see cref="PaymentTypes.FreeDailyExecDependant"/> only for Single and Tenfold<br/>
/// <see cref="PaymentTypes.FreeDailyTenfold"/> is <b>ALWAYS</b> Daily Free Tenfold
/// </summary>
public enum SummonExecTypes : byte
{
    Single = 1,
    Tenfold = 2,
    DailyDeal = 3
}
