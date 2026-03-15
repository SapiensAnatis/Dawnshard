namespace DragaliaAPI.Features.Wall;

public static class WallTimeProviderExtensions
{
    public static DateTimeOffset GetLastWallRewardDate(this TimeProvider timeProvider)
    {
        DateTimeOffset lastReset = timeProvider.GetLastDailyReset();

        int month;
        int year = lastReset.Year;

        month = lastReset.Day >= 15 ? lastReset.Month : lastReset.Month - 1;

        // Don't form an invalid date if we're in January
        if (month == 0)
        {
            month = 12;
            year -= 1;
        }

        return new(
            year,
            month,
            15,
            lastReset.Hour,
            lastReset.Minute,
            lastReset.Second,
            lastReset.Offset
        );
    }
}
