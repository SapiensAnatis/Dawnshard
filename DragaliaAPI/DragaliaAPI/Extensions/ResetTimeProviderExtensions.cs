namespace DragaliaAPI.Extensions;

public static class ResetTimeProviderExtensions
{
    /// <summary>
    /// Gets the last daily reset (6AM UTC of the previous/current day).
    /// </summary>
    public static DateTimeOffset GetLastDailyReset(this TimeProvider timeProvider) =>
        timeProvider.GetUtcNow().AddHours(-6).UtcDateTime.Date.AddHours(6);

    /// <summary>
    /// Gets the last weekly reset (6AM UTC of the previous Monday).
    /// </summary>
    public static DateTimeOffset GetLastWeeklyReset(this TimeProvider timeProvider)
    {
        DateTimeOffset lastDaily = timeProvider.GetLastDailyReset();
        int diff = DayOfWeek.Monday - lastDaily.DayOfWeek;
        return lastDaily.AddDays(diff > 0 ? diff - 7 : diff);
    }

    /// <summary>
    /// Gets the last monthly reset (6AM UTC of the 1st of the current month).
    /// </summary>
    public static DateTimeOffset GetLastMonthlyReset(this TimeProvider timeProvider)
    {
        DateTimeOffset lastDaily = timeProvider.GetLastDailyReset();
        return new DateTimeOffset(lastDaily.Year, lastDaily.Month, 1, 6, 0, 0, TimeSpan.Zero);
    }
}
