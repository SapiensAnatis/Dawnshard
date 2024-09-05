namespace DragaliaAPI.Features.Web.TimeAttack.Models;

internal sealed class TimeAttackRanking
{
    public int Rank { get; init; }

    public double Time { get; init; }

    public required IReadOnlyList<TimeAttackPlayer> Players { get; init; }
}
