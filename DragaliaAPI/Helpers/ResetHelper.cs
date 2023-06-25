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

    private DateTimeOffset GetLastDailyReset(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.Hour >= UtcHourReset
            ? dateTimeOffset.UtcDateTime.Date.AddHours(UtcHourReset)
            : dateTimeOffset.UtcDateTime.Date.AddDays(-1).AddHours(UtcHourReset);
    }

    /// <inheritdoc />
    public DateTimeOffset LastDailyReset => GetLastDailyReset(this.dateTimeProvider.UtcNow);

    /// <inheritdoc />
    public DateTimeOffset LastWeeklyReset
    {
        get
        {
            DateTimeOffset dateTimeOffset = dateTimeProvider.UtcNow;
            DateTimeOffset lastReset = GetLastDailyReset(dateTimeOffset);

            while (lastReset.DayOfWeek != DayOfWeek.Monday)
            {
                dateTimeOffset = dateTimeOffset.AddDays(-1);
                lastReset = GetLastDailyReset(dateTimeOffset);
            }

            return lastReset;
        }
    }

    /// <inheritdoc />
    public DateTimeOffset LastMonthlyReset
    {
        get
        {
            DateTimeOffset dateTimeOffset = dateTimeProvider.UtcNow;
            DateTimeOffset lastReset = GetLastDailyReset(dateTimeOffset);

            while (lastReset.Day != 1)
            {
                dateTimeOffset = dateTimeOffset.AddDays(-1);
                lastReset = GetLastDailyReset(dateTimeOffset);
            }

            return lastReset;
        }
    }
}
