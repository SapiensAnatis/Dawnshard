using System.Globalization;

namespace DragaliaAPI.Helpers;

public class ResetHelper : IResetHelper
{
    private readonly IDateTimeProvider dateTimeProvider;

    private const int UtcHourReset = 6;

    public ResetHelper(IDateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;

        CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        Thread.CurrentThread.CurrentCulture = culture;
    }

    /// <inheritdoc />
    public DateTimeOffset LastDailyReset =>
        this.dateTimeProvider.UtcNow.AddHours(-6).UtcDateTime.Date.AddHours(6);

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
