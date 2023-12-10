using System.Collections.Frozen;

namespace DragaliaAPI.Features.Shared.Options;

public class EventOptions
{
    public FrozenSet<EventRunInformation> EventList { get; init; } =
        FrozenSet<EventRunInformation>.Empty;
}
