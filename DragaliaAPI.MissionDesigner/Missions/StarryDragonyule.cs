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
                QuestStoryId = 2290306 // Final story ID; cannot progress with all 5 as _CompleteValue = 1
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
            // Defeat 2,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650402, EventId = EventId },
            // Defeat 5,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650403, EventId = EventId },
            // Defeat 7,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650404, EventId = EventId },
            // Defeat 10,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650405, EventId = EventId },
            // Defeat 12,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650406, EventId = EventId },
            // Defeat 15,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650407, EventId = EventId },
            // Defeat 17,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650408, EventId = EventId },
            // Defeat 20,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650409, EventId = EventId },
            // Defeat 25,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650410, EventId = EventId },
            // Defeat 30,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650411, EventId = EventId },
            // Clear an Invasion on Standard
            new EventRegularBattleClearMission()
            {
                MissionId = 11650501,
                EventId = EventId,
                VariationType = VariationTypes.Normal
            },
            // Clear an Invasion on Expert
            new EventRegularBattleClearMission()
            {
                MissionId = 11650601,
                EventId = EventId,
                VariationType = VariationTypes.Hard
            },
            // Collect 7,500 Heroism in One Invasion on Master
            new EventPointCollectionRecordMission()
            {
                MissionId = 11650701,
                EventId = EventId,
                VariationType = VariationTypes.VeryHard
            },
            // Clear a "One Starry Dragonyule" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 11650801,
                EventId = EventId,
                VariationType = VariationTypes.Normal
            },
            // Clear a "One Starry Dragonyule" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 11650901,
                EventId = EventId,
                VariationType = VariationTypes.Hard
            },
            // Clear a "One Starry Dragonyule" Trial on Master
            new EventTrialClearMission()
            {
                MissionId = 11651001,
                EventId = EventId,
                VariationType = VariationTypes.VeryHard
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
