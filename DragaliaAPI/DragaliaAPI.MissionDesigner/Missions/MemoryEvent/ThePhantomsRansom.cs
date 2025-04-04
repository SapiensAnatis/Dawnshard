/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class PhantomsRansom
{
    [MissionType(MissionType.MemoryEvent)]
    [EventId(20843)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10200101 },
            // Clear a Boss Battle
            new EventRegularBattleClearMission() { MissionId = 10200201 },
            // Clear a "The Phantom's Ransom" Quest with Free-Spirited Opera Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10200401,
                Crest = AbilityCrestId.FreeSpiritedOpera,
            },
            // Collect 100 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200501 },
            // Collect 500 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200502 },
            // Collect 1,500 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200503 },
            // Collect 4,000 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200504 },
            // Collect 6,000 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200505 },
            // Collect 7,000 Vibrato in One Go
            new EventPointCollectionRecordMission() { MissionId = 10200506 },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10200601 },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10200602 },
            // Clear 15 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10200603 },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10200604 },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10200605 },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200801 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200802 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200803 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200804 },
            // Clear 20 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200805 },
            // Clear 25 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10200806 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10200901,
                FullClear = true,
                VariationType = VariationTypes.VeryHard, // Expert maps to VeryHard
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10201001,
                FullClear = true,
                VariationType = VariationTypes.Extreme, // Master maps to Extreme
            },
            // Clear a "The Phantom's Ransom" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 10201101,
                VariationType = VariationTypes.Hell,
            },
            // Clear a "The Phantom's Ransom" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 10201201,
                VariationType = VariationTypes.Variation6,
            },
            // Earn the "Opera-Troupe Owner" Epithet
            new EventChallengeBattleClearMission() // Epithets are typically awarded for Master full clear
            {
                MissionId = 10201301,
                FullClear = true,
                VariationType = VariationTypes.Extreme, // Master maps to Extreme
            },
        ];
}
