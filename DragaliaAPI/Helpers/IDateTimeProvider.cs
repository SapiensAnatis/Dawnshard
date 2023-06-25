namespace DragaliaAPI.Helpers;

public interface IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; }
}
