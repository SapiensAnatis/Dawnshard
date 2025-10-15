/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class ADashOfDisaster
{
    [MissionType(MissionType.MemoryEvent)]
    [EventId(20841)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10180101 },
        // Clear a Boss Battle
        new EventRegularBattleClearMission() { MissionId = 10180201 },
        // Clear an "A Dash of Disaster" Quest with Berry Lovable Friends Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10180401,
            Crest = AbilityCrestId.BerryLovableFriends,
        },
        // Collect 100 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180501 },
        // Collect 500 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180502 },
        // Collect 1,500 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180503 },
        // Collect 4,000 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180504 },
        // Collect 6,000 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180505 },
        // Collect 7,000 Zest in One Go
        new EventPointCollectionRecordMission() { MissionId = 10180506 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10180601 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10180602 },
        // Clear 15 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10180603 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10180604 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10180605 },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180801 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180802 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180803 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180804 },
        // Clear 20 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180805 },
        // Clear 25 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10180806 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10180901,
            FullClear = true,
            VariationType = VariationTypes.VeryHard, // Difficulty 3 for Expert Challenge Battle
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10181001,
            FullClear = true,
            VariationType = VariationTypes.Extreme, // Difficulty 4 for Master Challenge Battle
        },
        // Earn the "Chef de Cuisine" Epithet
        // Typically awarded for completing the Master Challenge Battle with all endeavors
        new EventChallengeBattleClearMission()
        {
            MissionId = 10181301,
            FullClear = true,
            VariationType = VariationTypes.Extreme, // Linked to Master Challenge Battle Clear
        },
    ];
}
