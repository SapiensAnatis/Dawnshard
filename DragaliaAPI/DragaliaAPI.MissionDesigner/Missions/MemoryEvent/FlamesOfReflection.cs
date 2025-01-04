using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class FlamesOfReflection
{
    private const int ExBossBattleQuestId = 208160401;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(20816)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10010101 },
            //  Defeat the Troll Smith Twice
            new EventRegularBattleClearMission() { MissionId = 10010102 },
            // Clear a Flames of Reflection Quest with The Dragon Smiths Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10010201,
                Crest = AbilityCrestId.TheDragonSmiths,
            },
            // Collect 100 Mettle in One Go
            new EventPointCollectionRecordMission() { MissionId = 10010301 },
            // Collect 500 Mettle in One Go
            new EventPointCollectionRecordMission() { MissionId = 10010302 },
            // Collect 1,500 Mettle in One Go
            new EventPointCollectionRecordMission() { MissionId = 10010303 },
            // Collect 4,000 Mettle in One Go
            new EventPointCollectionRecordMission() { MissionId = 10010304 },
            // Collect 7,000 Mettle in One Go
            new EventPointCollectionRecordMission() { MissionId = 10010305 },
            // Clear a Boss Battle Five Times
            new EventRegularBattleClearMission() { MissionId = 10010502 },
            // Clear a Boss Battle 10 Times
            new EventRegularBattleClearMission() { MissionId = 10010503 },
            // Clear a Boss Battle 20 Times
            new EventRegularBattleClearMission() { MissionId = 10010504 },
            // Clear a Boss Battle 30 Times
            new EventRegularBattleClearMission() { MissionId = 10010505 },
            // Clear Three Extra Boss Battles
            new ClearQuestMission() { MissionId = 10010601, QuestId = ExBossBattleQuestId },
            // Clear Six Extra Boss Battles
            new ClearQuestMission() { MissionId = 10010602, QuestId = ExBossBattleQuestId },
            // Clear 10 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10010603, QuestId = ExBossBattleQuestId },
            // Clear 15 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10010604, QuestId = ExBossBattleQuestId },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10010701 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10010702 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10010703 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10010704 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10010801,
                VariationType = VariationTypes.VeryHard,
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10010901,
                VariationType = VariationTypes.Extreme,
            },
            // Earn the "Smithing Legend" Epithet
            new EventChallengeBattleClearMission()
            {
                MissionId = 10011001,
                VariationType = VariationTypes.Extreme,
            },
        ];
}
