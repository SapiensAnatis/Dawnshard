using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class TheHuntForHarmony
{
    // The Vernal Games: Finals
    private const int ExBossBattleId = 208220401;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(20822)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10050101 },
        //  Defeat the Volcanic Cyclops Twice
        new EventRegularBattleClearMission() { MissionId = 10050102 },
        // Clear a "The Hunt for Harmony" Quest with A Mother's Love Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10050201,
            Crest = AbilityCrestId.AMothersLove,
        },
        // Collect 100 Eggsploration Points in One Go
        new EventPointCollectionRecordMission() { MissionId = 10050301 },
        // Collect 500 Eggsploration Points in One Go
        new EventPointCollectionRecordMission() { MissionId = 10050302 },
        // Collect 1,500 Eggsploration Points in One Go
        new EventPointCollectionRecordMission() { MissionId = 10050303 },
        // Collect 4,000 Eggsploration Points in One Go
        new EventPointCollectionRecordMission() { MissionId = 10050304 },
        // Collect 7,000 Eggsploration Points in One Go
        new EventPointCollectionRecordMission() { MissionId = 10050305 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10050401 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10050402 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10050403 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10050404 },
        // Clear Three Extra Boss Battles
        new ClearQuestMission() { MissionId = 10050501, QuestId = ExBossBattleId },
        // Clear Six Extra Boss Battles
        new ClearQuestMission() { MissionId = 10050502, QuestId = ExBossBattleId },
        // Clear 10 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10050503, QuestId = ExBossBattleId },
        // Clear 15 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10050504, QuestId = ExBossBattleId },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10050601 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10050602 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10050603 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10050604 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10050701,
            FullClear = true,
            VariationType = VariationTypes.VeryHard,
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10050801,
            FullClear = true,
            VariationType = VariationTypes.Extreme,
        },
        // Earn the "Wish Realizer" Epithet
        new EventChallengeBattleClearMission()
        {
            MissionId = 10050901,
            FullClear = true,
            VariationType = VariationTypes.Extreme,
        },
    ];
}
