using System.Collections.Frozen;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Helpers;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Login;

[UsedImplicitly]
public class DailyEndeavourResetAction(
    IOptionsMonitor<EventOptions> eventOptionsMonitor,
    IMissionRepository missionRepository,
    IResetHelper resetHelper,
    IMissionService missionService,
    IEventRepository eventRepository,
    ILogger<DailyEndeavourResetAction> logger
) : IDailyResetAction
{
    private readonly EventOptions eventOptions = eventOptionsMonitor.CurrentValue;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly IResetHelper resetHelper = resetHelper;
    private readonly IMissionService missionService = missionService;
    private readonly IEventRepository eventRepository = eventRepository;
    private readonly ILogger<DailyEndeavourResetAction> logger = logger;

    private static readonly FrozenSet<DailyMission> PermanentDailyMissions = MasterAsset
        .DailyMission.Enumerable.Where(x => x.Id is >= 15070101 and <= 15070601) // https://dragalialost.wiki/w/Endeavors#Daily_Endeavors
        .ToFrozenSet();

    public async Task Apply()
    {
        await missionRepository.ClearDailyMissions();

        this.logger.LogDebug("Adding permanent daily endeavours");

        DateTimeOffset lastDailyReset = this.resetHelper.LastDailyReset;

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

        IEnumerable<EventRunInformation> activeEvents = this.eventOptions.EventList.Where(
            x => lastDailyReset > x.Start && lastDailyReset < x.End
        );

        foreach (EventRunInformation activeEvent in activeEvents)
        {
            this.logger.LogDebug("Adding daily endeavours for event {eventId}", activeEvent.Id);

            if (!await this.eventRepository.HasEventDataAsync(activeEvent.Id))
            {
                this.logger.LogDebug("Skipping as event data uninitialized");
                continue;
            }

            IEnumerable<DailyMission> eventDailyMissions =
                MasterAsset.DailyMission.Enumerable.Where(x => x.QuestGroupId == activeEvent.Id);

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
}
