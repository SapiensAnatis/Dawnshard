namespace DragaliaAPI.Features.Shared.Options;

public class EventRunInformation
{
    public int Id { get; init; }

    public DateTimeOffset Start { get; init; }

    public DateTimeOffset End { get; init; }

    public bool IsActive(TimeProvider timeProvider) =>
        timeProvider.GetLastDailyReset() > this.Start
        && timeProvider.GetLastDailyReset() < this.End;
}
