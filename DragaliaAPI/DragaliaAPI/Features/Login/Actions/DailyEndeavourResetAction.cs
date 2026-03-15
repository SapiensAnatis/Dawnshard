using System.Collections.Frozen;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Login.Actions;

[UsedImplicitly]
public partial class DailyEndeavourResetAction(
    IOptionsMonitor<EventOptions> eventOptionsMonitor,
    IMissionRepository missionRepository,
    TimeProvider timeProvider,
    IMissionService missionService,
    IEventRepository eventRepository,
    ILogger<DailyEndeavourResetAction> logger
) : IDailyResetAction
{
    private readonly EventOptions eventOptions = eventOptionsMonitor.CurrentValue;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly TimeProvider timeProvider = timeProvider;
    private readonly IMissionService missionService = missionService;
    private readonly IEventRepository eventRepository = eventRepository;
    private readonly ILogger<DailyEndeavourResetAction> logger = logger;

    private static readonly FrozenSet<DailyMission> PermanentDailyMissions = MasterAsset
        .MissionDailyData.Enumerable.Where(x => x.Id is >= 15070101 and <= 15070601) // https://dragalialost.wiki/w/Endeavors#Daily_Endeavors
        .ToFrozenSet();

    public async Task Apply()
    {
        await this.missionRepository.ClearDailyMissions();

        Log.AddingPermanentDailyEndeavours(this.logger);

        DateTimeOffset lastDailyReset = this.timeProvider.GetLastDailyReset();

        foreach (DailyMission permanentDaily in PermanentDailyMissions)
        {
            await this.missionService.StartMission(
                MissionType.Daily,
                permanentDaily.Id,
                groupId: 0,
                startTime: lastDailyReset,
                endTime: lastDailyReset.AddDays(1)
            );
        }

        IEnumerable<EventRunInformation> activeEvents = this.eventOptions.EventList.Where(x =>
            lastDailyReset > x.Start && lastDailyReset < x.End
        );

        foreach (EventRunInformation activeEvent in activeEvents)
        {
            Log.AddingDailyEndeavoursForEvent(this.logger, activeEvent.Id);

            if (!await this.eventRepository.HasEventDataAsync(activeEvent.Id))
            {
                Log.SkippingAsEventDataUninitialized(this.logger);
                continue;
            }

            IEnumerable<DailyMission> eventDailyMissions =
                MasterAsset.MissionDailyData.Enumerable.Where(x =>
                    x.QuestGroupId == activeEvent.Id
                );

            foreach (DailyMission eventDaily in eventDailyMissions)
            {
                await this.missionService.StartMission(
                    MissionType.Daily,
                    eventDaily.Id,
                    groupId: activeEvent.Id,
                    startTime: lastDailyReset,
                    endTime: lastDailyReset.AddDays(1)
                );
            }
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Adding permanent daily endeavours")]
        public static partial void AddingPermanentDailyEndeavours(ILogger logger);

        [LoggerMessage(LogLevel.Debug, "Adding daily endeavours for event {eventId}")]
        public static partial void AddingDailyEndeavoursForEvent(ILogger logger, int eventId);

        [LoggerMessage(LogLevel.Debug, "Skipping as event data uninitialized")]
        public static partial void SkippingAsEventDataUninitialized(ILogger logger);
    }
}
