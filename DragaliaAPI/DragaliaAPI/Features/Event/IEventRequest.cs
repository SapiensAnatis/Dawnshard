namespace DragaliaAPI.Features.Event;

/// <summary>
/// Represents a request that pertains to a particular event.
/// </summary>
public interface IEventRequest
{
    public int EventId { get; }
}
