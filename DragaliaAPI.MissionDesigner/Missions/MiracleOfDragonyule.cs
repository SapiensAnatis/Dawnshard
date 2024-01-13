using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class MiracleOfDragonyule
{
    private const int EventId = 20817;
    private const int Ep5StoryId = 2081705;
    private const int ExBossBattleQuestId = 208170401;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10020101 },
            // Defeat the Festive Treant
            new EventRegularBattleClearMission() { MissionId = 10020102 },
            // Defeat the Festive Treant Two Times
            new EventRegularBattleClearMission() { MissionId = 10020103 },
            // Defeat the Festive Treant Three Times
            new EventRegularBattleClearMission() { MissionId = 10020104 },
            // Clear a "The Miracle of Dragonyule" Quest with A Slice of Dragonyule Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10020201,
                Crest = AbilityCrests.ASliceofDragonyule
            },
            // Collect 500 Holiday Cheer in One Go
            new EventPointCollectionRecordMission() { MissionId = 10020302 },
            // Collect 1,500 Holiday Cheer in One Go
            new EventPointCollectionRecordMission() { MissionId = 10020303 },
            // Collect 4,000 Holiday Cheer in One Go
            new EventPointCollectionRecordMission() { MissionId = 10020304 },
            // Collect 7,000 Holiday Cheer in One Go
            new EventPointCollectionRecordMission() { MissionId = 10020305 },
            // Read Episode 5 of the Event Story
            new ReadQuestStoryMission() { MissionId = 10020401, QuestStoryId = Ep5StoryId },
            // Clear a Boss Battle Five Times
            new EventRegularBattleClearMission() { MissionId = 10020502 },
            // Clear a Boss Battle 10 Times
            new EventRegularBattleClearMission() { MissionId = 10020503 },
            // Clear a Boss Battle 20 Times
            new EventRegularBattleClearMission() { MissionId = 10020504 },
            // Clear a Boss Battle 30 Times
            new EventRegularBattleClearMission() { MissionId = 10020505 },
            // Clear Three Extra Boss Battles
            new ClearQuestMission() { MissionId = 10020601, QuestId = ExBossBattleQuestId },
            // Clear Six Extra Boss Battles
            new ClearQuestMission() { MissionId = 10020602, QuestId = ExBossBattleQuestId },
            // Clear 10 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10020603, QuestId = ExBossBattleQuestId },
            // Clear 15 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10020604, QuestId = ExBossBattleQuestId },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10020702 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10020703 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10020704 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10020801,
                VariationType = VariationTypes.Normal,
                FullClear = true
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10020901,
                VariationType = VariationTypes.Extreme,
                FullClear = true
            },
            // Earn the "The True Saint Starfall" Epithet
            new EventChallengeBattleClearMission()
            {
                MissionId = 10021001,
                VariationType = VariationTypes.Extreme,
                FullClear = true
            },
        ];
}
