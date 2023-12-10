using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using JetBrains.Annotations;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
[UsedImplicitly]
public static class StarryDragonyule
{
    private const int EventId = 22903;
    private const int DailyProgressionGroupId = 2290301;

    [MissionList(Type = MissionType.Daily)]
    [UsedImplicitly]
    public static List<Mission> DailyMissions { get; } =

        [
            // Collect 1,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190101,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 1,500 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190102,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 2,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190103,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 2,500 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190104,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 3,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190105,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Clear an Invasion
            new EventRegularBattleClearMission()
            {
                MissionId = 11190201,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Clear Five Invasions
            new EventRegularBattleClearMission()
            {
                MissionId = 11190202,
                EventId = EventId,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Clear All Daily Event Endeavours
            new ClearProgressionGroupMission()
            {
                MissionId = 11190301,
                ProgressionGroupToClear = DailyProgressionGroupId,
            }
        ];
}
