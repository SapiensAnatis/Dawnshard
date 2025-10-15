/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class CrescendoOfCourage
{
    // Quest IDs from QuestNames.txt
    private const int ExtraBossBattleId = 208290401; // A Fiendish Encore

    [MissionType(MissionType.MemoryEvent)]
    [EventId(20829)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10070101 },
        // Clear a Boss Battle
        new EventRegularBattleClearMission() { MissionId = 10070102 },
        // Clear an "A Crescendo of Courage" Quest with Surfing Siblings Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10070201,
            Crest = AbilityCrestId.SurfingSiblings,
        },
        // Collect 100 Hype in One Go
        new EventPointCollectionRecordMission() { MissionId = 10070301 },
        // Collect 500 Hype in One Go
        new EventPointCollectionRecordMission() { MissionId = 10070302 },
        // Collect 1,500 Hype in One Go
        new EventPointCollectionRecordMission() { MissionId = 10070303 },
        // Collect 4,000 Hype in One Go
        new EventPointCollectionRecordMission() { MissionId = 10070304 },
        // Collect 7,000 Hype in One Go
        new EventPointCollectionRecordMission() { MissionId = 10070305 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10070401 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10070402 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10070403 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10070404 },
        // Clear Three Extra Boss Battles
        new ClearQuestMission() { MissionId = 10070501, QuestId = ExtraBossBattleId },
        // Clear Six Extra Boss Battles
        new ClearQuestMission() { MissionId = 10070502, QuestId = ExtraBossBattleId },
        // Clear 10 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10070503, QuestId = ExtraBossBattleId },
        // Clear 15 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10070504, QuestId = ExtraBossBattleId },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10070601 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10070602 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10070603 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10070604 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10070701,
            FullClear = true,
            VariationType = VariationTypes.VeryHard, // Difficulty 3 for Expert Challenge Battle
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10070801,
            FullClear = true,
            VariationType = VariationTypes.Extreme, // Difficulty 4 for Master Challenge Battle
        },
        // Earn the "Summer Champion" Epithet
        // Typically awarded for completing the Master Challenge Battle with all endeavors
        new EventChallengeBattleClearMission()
        {
            MissionId = 10070901,
            FullClear = true,
            VariationType = VariationTypes.Extreme, // Linked to Master Challenge Battle Clear
        },
    ];
}
