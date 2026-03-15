using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Start missions for players who have already created memory event data.
/// </summary>
[UsedImplicitly]
public partial class V13Update(
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

            Log.UnlockingMissionsForMemoryEvent(logger, eventId);
            await missionService.UnlockMemoryEventMissions(eventId);
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Unlocking missions for memory event {eventId}")]
        public static partial void UnlockingMissionsForMemoryEvent(ILogger logger, int eventId);
    }
}
