namespace DragaliaAPI.Helpers;

[Obsolete("Use .NET 8's TimeProvider class instead")]
public interface IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; }
}
