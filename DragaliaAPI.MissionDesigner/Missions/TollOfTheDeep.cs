using DragaliaAPI.MissionDesigner.Models;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class TollOfTheDeep
{
    private const int EventId = 20845;

    [MissionList(Type = MissionType.MemoryEvent)]
    public static List<Mission> Missions { get; } =
        new()
        {
            // Participate In The Event
            new EventParticipationMission { MissionId = 10220101, EventId = EventId, }
        };
}
