namespace DragaliaAPI.Helpers;

public class DateTimeProvider : IDateTimeProvider
{
    // So that it always returns the same time
    public DateTimeOffset UtcNow { get; } = DateTimeOffset.UtcNow;
}
