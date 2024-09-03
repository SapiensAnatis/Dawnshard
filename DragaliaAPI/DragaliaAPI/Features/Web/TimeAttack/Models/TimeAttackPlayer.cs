namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed class TimeAttackPlayer
{
    public required string Name { get; init; }

    public required IReadOnlyList<TimeAttackUnit> Units { get; init; }
}
