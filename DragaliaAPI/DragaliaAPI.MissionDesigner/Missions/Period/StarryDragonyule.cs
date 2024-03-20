using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class StarryDragonyule
{
    private const int EventId = 22903;
    private const int DailyProgressionGroupId = 2290301;

    [MissionType(MissionType.Period)]
    [EventId(EventId)]
    public static List<Mission> PeriodMissions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 11650101 },
            // Clear All of the Event's Story Quests
            new ReadQuestStoryMission()
            {
                MissionId = 11650201,
                QuestStoryId = 2290306 // Final story ID; cannot progress with all 5 as _CompleteValue = 1
            },
            // Collect 1,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650301 },
            // Collect 2,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650302 },
            // Collect 3,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650303 },
            // Collect 4,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650304, },
            // Collect 5,000 Heroism in One Invasion
            new EventPointCollectionRecordMission() { MissionId = 11650305, },
            // Defeat 1,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650401, },
            // Defeat 2,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650402, },
            // Defeat 5,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650403, },
            // Defeat 7,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650404 },
            // Defeat 10,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650405 },
            // Defeat 12,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650406 },
            // Defeat 15,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650407 },
            // Defeat 17,500 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650408 },
            // Defeat 20,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650409 },
            // Defeat 25,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650410 },
            // Defeat 30,000 Enemies in Invasions
            new EarnEnemiesKilledMission() { MissionId = 11650411 },
            // Clear an Invasion on Standard
            new EventRegularBattleClearMission()
            {
                MissionId = 11650501,
                VariationType = VariationTypes.Normal
            },
            // Clear an Invasion on Expert
            new EventRegularBattleClearMission()
            {
                MissionId = 11650601,
                VariationType = VariationTypes.Hard
            },
            // Collect 7,500 Heroism in One Invasion on Master
            new EventPointCollectionRecordMission()
            {
                MissionId = 11650701,
                VariationType = VariationTypes.VeryHard
            },
            // Clear a "One Starry Dragonyule" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 11650801,
                VariationType = VariationTypes.Normal
            },
            // Clear a "One Starry Dragonyule" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 11650901,
                VariationType = VariationTypes.Hard
            },
            // Clear a "One Starry Dragonyule" Trial on Master
            new EventTrialClearMission()
            {
                MissionId = 11651001,
                VariationType = VariationTypes.VeryHard
            },
            // Clear A Dragonyule Miracle
            new ClearQuestMission() { MissionId = 11651101, QuestId = 229030401, }
        ];

    [MissionType(MissionType.Daily)]
    [EventId(EventId)]
    public static List<Mission> DailyMissions { get; } =
        [
            // Collect 1,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190101,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 1,500 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190102,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 2,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190103,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 2,500 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190104,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Collect 3,000 Heroism
            new EventPointCollectionMission()
            {
                MissionId = 11190105,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Clear an Invasion
            new EventRegularBattleClearMission()
            {
                MissionId = 11190201,
                ProgressionGroupId = DailyProgressionGroupId
            },
            // Clear Five Invasions
            new EventRegularBattleClearMission()
            {
                MissionId = 11190202,
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
