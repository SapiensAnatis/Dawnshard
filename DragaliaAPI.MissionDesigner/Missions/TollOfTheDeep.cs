using DragaliaAPI.MissionDesigner.Models;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class TollOfTheDeep
{
    [MissionList(Type = MissionType.MemoryEvent)]
    public static List<Mission> Missions { get; } =
        new()
        {
            // Participate In The Event
            new EventParticipationMission { MissionId = 10220101, EventId = 20845, }
        };
}
