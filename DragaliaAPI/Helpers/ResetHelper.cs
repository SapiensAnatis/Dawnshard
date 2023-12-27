namespace DragaliaAPI.Helpers;

public class ResetHelper(TimeProvider timeProvider) : IResetHelper
{
    private readonly TimeProvider timeProvider = timeProvider;

    private const int UtcHourReset = 6;

    /// <inheritdoc />
    public DateTimeOffset LastDailyReset =>
        this.timeProvider.GetUtcNow().AddHours(-6).UtcDateTime.Date.AddHours(6);

    public DateTimeOffset UtcNow => this.timeProvider.GetUtcNow();

    /// <inheritdoc />
    public DateTimeOffset LastWeeklyReset
    {
        get
        {
            DateTimeOffset lastDaily = LastDailyReset;
            int diff = DayOfWeek.Monday - lastDaily.DayOfWeek;
            return lastDaily.AddDays(diff > 0 ? diff - 7 : diff);
        }
    }

    /// <inheritdoc />
    public DateTimeOffset LastMonthlyReset
    {
        get
        {
            DateTimeOffset lastDaily = LastDailyReset;

            return new DateTimeOffset(lastDaily.Year, lastDaily.Month, 1, 6, 0, 0, TimeSpan.Zero);
        }
    }
}
