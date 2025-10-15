/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class AgentsOfTheGoddess
{
    [MissionType(MissionType.MemoryEvent)]
    [EventId(20839)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10130101 },
        // Clear a Boss Battle
        new EventRegularBattleClearMission() { MissionId = 10130201 },
        // Clear an Agents of the Goddess Quest with Wings in the Night Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10130401,
            Crest = AbilityCrestId.WingsintheNight,
        },
        // Collect 100 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130501 },
        // Collect 500 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130502 },
        // Collect 1,500 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130503 },
        // Collect 4,000 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130504 },
        // Collect 6,000 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130505 },
        // Collect 7,000 Sanctity in One Go
        new EventPointCollectionRecordMission() { MissionId = 10130506 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10130601 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10130602 },
        // Clear 15 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10130603 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10130604 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10130605 },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130801 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130802 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130803 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130804 },
        // Clear 20 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130805 },
        // Clear 25 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10130806 },
        // Completely Clear a Challenge Battle on Expert
        // Quest 208390501 "Divine Deliverance: Expert" has difficulty 3 (VeryHard)
        new EventChallengeBattleClearMission()
        {
            MissionId = 10130901,
            FullClear = true,
            VariationType = VariationTypes.VeryHard,
        },
        // Completely Clear a Challenge Battle on Master
        // Quest 208390502 "Divine Deliverance: Master" has difficulty 4 (Extreme)
        new EventChallengeBattleClearMission()
        {
            MissionId = 10131001,
            FullClear = true,
            VariationType = VariationTypes.Extreme,
        },
        // Clear an "Agents of the Goddess" Trial on Standard
        new ClearQuestMission()
        {
            // Human note: the trials for this one use a non-standard quest ID and don't work with the
            // usual progression type. Rather than widen the detection for trial clears, and risk false
            // positives, hardcode these as normal 'clear quest' missions
            MissionId = 10131101,
            QuestId = 208390304,
        },
        // Clear an "Agents of the Goddess" Trial on Expert
        new ClearQuestMission() { MissionId = 10131201, QuestId = 208390305 },
        // Earn the "Goddess's Proxy" Epithet
        // Typically awarded for completing the Master Challenge Battle with all endeavors.
        new EventChallengeBattleClearMission()
        {
            MissionId = 10131301,
            FullClear = true,
            VariationType = VariationTypes.Extreme, // Linked to Master Challenge Battle Clear
        },
    ];
}
