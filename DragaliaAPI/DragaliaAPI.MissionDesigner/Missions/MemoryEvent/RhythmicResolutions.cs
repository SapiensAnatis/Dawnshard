using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class RhythmicResolutions
{
    [MissionType(MissionType.MemoryEvent)]
    [EventId(20842)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10190101 },
        // Clear a Boss Battle
        new EventRegularBattleClearMission() { MissionId = 10190201 },
        // Clear a Rhythmic Resolutions Quest with Best Buds Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10190401,
            Crest = AbilityCrestId.BestBuds,
        },
        // Collect 100 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190501 },
        // Collect 500 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190502 },
        // Collect 1,500 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190503 },
        // Collect 4,000 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190504 },
        // Collect 6,000 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190505 },
        // Collect 7,000 Festival Spirit in One Go
        new EventPointCollectionRecordMission() { MissionId = 10190506 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10190601 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10190602 },
        // Clear 15 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10190603 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10190604 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10190605 },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190801 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190802 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190803 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190804 },
        // Clear 20 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190805 },
        // EventChallengeBattleClearMission 25 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10190806 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10190901,
            VariationType = VariationTypes.VeryHard,
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10191001,
            VariationType = VariationTypes.Extreme,
        },
        // Earn the "Festival Fanatic" Epithet
        new EventChallengeBattleClearMission()
        {
            MissionId = 10191301,
            VariationType = VariationTypes.Extreme,
        },
    ];
}
