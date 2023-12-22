using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Start missions for players who have already created memory event data.
/// </summary>
[UsedImplicitly]
public class V13Update(
    IEventRepository eventRepository,
    IMissionService missionService,
    ILogger<V13Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 13;

    public async Task Apply()
    {
        int[] startedEventIds = await eventRepository
            .EventData.Select(x => x.EventId)
            .Distinct()
            .ToArrayAsync();

        foreach (int eventId in startedEventIds)
        {
            if (
                !MasterAsset.EventData.TryGetValue(eventId, out EventData? eventData)
                || !eventData.IsMemoryEvent
            )
            {
                continue;
            }

            logger.LogDebug("Unlocking missions for memory event {eventId}", eventId);
            await missionService.UnlockMemoryEventMissions(eventId);
        }
    }
}
