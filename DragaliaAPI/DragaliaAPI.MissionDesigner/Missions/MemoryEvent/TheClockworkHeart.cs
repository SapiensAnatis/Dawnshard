using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class TheClockworkHeart
{
    private const int EventId = 20846;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10230101 },
        // Clear a Boss Battle
        new EventRegularBattleClearMission() { MissionId = 10230201 },
        // Clear a "The Clockwork Heart" Quest with A Halloween Spectacular! Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10230401,
            Crest = AbilityCrestId.AHalloweenSpectacular,
        },
        // Collect 100 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230501 },
        // Collect 500 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230502 },
        // Collect 1,500 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230503 },
        // Collect 4,000 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230504 },
        // Collect 6,000 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230505 },
        // Collect 7,000 Acclaim in One Go
        new EventPointCollectionRecordMission() { MissionId = 10230506 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10230601 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10230602 },
        // Clear 15 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10230603 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10230604 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10230605 },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230801 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230802 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230803 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230804 },
        // Clear 20 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230805 },
        // Clear 25 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10230806 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10230901,
            FullClear = true,
            VariationType = VariationTypes.VeryHard,
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10231001,
            FullClear = true,
            VariationType = VariationTypes.Extreme,
        },
        // Clear a "The Clockwork Heart" Trial on Standard
        new EventTrialClearMission() { MissionId = 10231101, VariationType = VariationTypes.Hell },
        // Clear a "The Clockwork Heart" Trial on Expert
        new EventTrialClearMission()
        {
            MissionId = 10231201,
            VariationType = VariationTypes.Variation6,
        },
        // Earn the "Legendary Actress" Epithet
        new EventChallengeBattleClearMission()
        {
            MissionId = 10231301,
            FullClear = true,
            VariationType = VariationTypes.Extreme,
        },
    ];
}
