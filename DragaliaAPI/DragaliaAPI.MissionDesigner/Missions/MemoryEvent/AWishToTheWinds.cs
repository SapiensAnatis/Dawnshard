/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
// Named based on the mission content "A Wish to the Winds"
public static class AWishToTheWinds
{
    // Quest IDs from QuestNames.txt
    private const int ExtraBossBattleId = 208180401; // Mega-Fiend Spotted!

    [MissionType(MissionType.MemoryEvent)]
    [EventId(20818)] // Event ID derived from QuestNames.txt IDs
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10030101 },
            // Defeat the Water Troll King Twice
            // Typically tracked as standard boss clears in this system.
            new EventRegularBattleClearMission() { MissionId = 10030102 },
            // Clear an A Wish to the Winds Quest with Louise's Hobbies Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10030201,
                Crest = AbilityCrestId.LouisesHobbies,
            },
            // Collect 100 Divine Gales in One Go
            new EventPointCollectionRecordMission() { MissionId = 10030301 },
            // Collect 500 Divine Gales in One Go
            new EventPointCollectionRecordMission() { MissionId = 10030302 },
            // Collect 1,500 Divine Gales in One Go
            new EventPointCollectionRecordMission() { MissionId = 10030303 },
            // Collect 4,000 Divine Gales in One Go
            new EventPointCollectionRecordMission() { MissionId = 10030304 },
            // Collect 7,000 Divine Gales in One Go
            new EventPointCollectionRecordMission() { MissionId = 10030305 },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10030502 },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10030503 },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10030504 },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10030505 },
            // Clear Three Extra Boss Battles
            new ClearQuestMission() { MissionId = 10030601, QuestId = ExtraBossBattleId },
            // Clear Six Extra Boss Battles
            new ClearQuestMission() { MissionId = 10030602, QuestId = ExtraBossBattleId },
            // Clear 10 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10030603, QuestId = ExtraBossBattleId },
            // Clear 15 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10030604, QuestId = ExtraBossBattleId },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10030701 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10030702 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10030703 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10030704 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10030801,
                FullClear = true,
                VariationType = VariationTypes.Normal,
            },
            // Completely Clear a Challenge Battle on Master
            // QuestNames lists "Ruler of the Shore: Master" as difficulty 4 (Extreme).
            new EventChallengeBattleClearMission()
            {
                MissionId = 10030901,
                FullClear = true,
                VariationType = VariationTypes.Extreme,
            },
            // Earn the "Wind Wisher" Epithet
            // Typically awarded for completing the Master Challenge Battle with all endeavors.
            new EventChallengeBattleClearMission()
            {
                MissionId = 10031001,
                FullClear = true,
                VariationType = VariationTypes.Extreme, // Linked to Master Challenge Battle Clear
            },
        ];
}
