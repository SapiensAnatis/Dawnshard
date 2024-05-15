namespace DragaliaAPI.Features.Wall;

public static class WallTimeProviderExtensions
{
    public static DateTimeOffset GetLastWallRewardDate(this TimeProvider timeProvider)
    {
        DateTimeOffset lastReset = timeProvider.GetLastDailyReset();

        int month;

        if (lastReset.Day >= 15)
        {
            month = lastReset.Month;
        }
        else
        {
            month = lastReset.Month - 1;
        }

        return new DateTimeOffset(
            lastReset.Year,
            month,
            15,
            lastReset.Hour,
            lastReset.Minute,
            lastReset.Second,
            lastReset.Offset
        );
    }
}
