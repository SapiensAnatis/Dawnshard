namespace DragaliaAPI.Helpers;

public interface IResetHelper
{
    /// <summary>
    /// Gets the last daily reset (6AM UTC of the previous/current day).
    /// </summary>
    DateTimeOffset LastDailyReset { get; }

    /// <summary>
    /// Gets the last weekly reset (6AM UTC of the previous Monday).
    /// </summary>
    DateTimeOffset LastWeeklyReset { get; }

    /// <summary>
    /// Gets the last monthly reset (6AM UTC of the 1st of the current month).
    /// </summary>
    DateTimeOffset LastMonthlyReset { get; }
}
