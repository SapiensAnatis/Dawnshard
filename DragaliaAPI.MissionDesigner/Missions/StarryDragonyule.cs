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

    [MissionList(Type = MissionType.Period)]
    [UsedImplicitly]
    public static List<Mission> PeriodMissions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 11650101, EventId = EventId },
            // Clear All of the Event's Story Quests
            new ReadQuestStoryMission()
            {
                MissionId = 11650201,
                QuestStoryId = 11650201 // Final story ID; cannot progress with all 5 as _CompleteValue = 1
            },
            // Collect 1,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650301, EventId = EventId },
            // Collect 2,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650302, EventId = EventId },
            // Collect 3,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650303, EventId = EventId },
            // Collect 4,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650304, EventId = EventId },
            // Collect 5,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650305, EventId = EventId },
            // Defeat 1,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650401, EventId = EventId },
            // Clear an Invasion on Standard
            new ClearQuestMission() { MissionId = 11650501, QuestId = 229031201 },
            // Clear an Invasion on Expert
            new ClearQuestMission() { MissionId = 11650601, QuestId = 229031202 },
            // Collect 7,500 Heroism in One Invasion on Master
            new EventPointCollectionRecordMission()
            {
                MissionId = 11650701,
                EventId = EventId,
                QuestId = 229031203
            },
            // Clear a "One Starry Dragonyule" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 11650801,
                QuestId = 229031301,
                EventId = EventId
            },
            // Clear a "One Starry Dragonyule" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 11650901,
                EventId = EventId,
                QuestId = 229031302,
            },
            // Clear a "One Starry Dragonyule" Trial on Master
            new EventTrialClearMission()
            {
                MissionId = 11650001,
                EventId = EventId,
                QuestId = 229031303,
            },
            // Clear A Dragonyule Miracle
            new ClearQuestMission() { MissionId = 11651101, QuestId = 229030401, }
        ];

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
