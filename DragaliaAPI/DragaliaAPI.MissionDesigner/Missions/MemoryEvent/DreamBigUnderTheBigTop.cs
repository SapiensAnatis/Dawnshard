using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class DreamBigUnderTheBigTop
{
    private const int EventId = 20820;
    private const int ExBossBattleQuestId = 208200401;
    private const int ExpertChallengeBattleQuestId = 208200501;
    private const int MasterChallengeBattleQuestId = 208200501;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10040101 },
        //  Defeat the Manticore Twice
        new EventRegularBattleClearMission() { MissionId = 10040102 },
        // Clear a "Dream Big Under the Big Top" Quest with Astounding Trick Equipped
        new EventQuestClearWithCrestMission()
        {
            MissionId = 10040201,
            Crest = AbilityCrestId.AstoundingTrick,
        },
        // Collect 100 Renown in One Go
        new EventPointCollectionRecordMission() { MissionId = 10040301 },
        // Collect 500 Renown in One Go
        new EventPointCollectionRecordMission() { MissionId = 10040302 },
        // Collect 1,500 Renown in One Go
        new EventPointCollectionRecordMission() { MissionId = 10040303 },
        // Collect 4,000 Renown in One Go
        new EventPointCollectionRecordMission() { MissionId = 10040304 },
        // Collect 7,000 Renown in One Go
        new EventPointCollectionRecordMission() { MissionId = 10040305 },
        // Clear Five Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10040501 },
        // Clear 10 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10040502 },
        // Clear 20 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10040503 },
        // Clear 30 Boss Battles
        new EventRegularBattleClearMission() { MissionId = 10040504 },
        // Clear Three Extra Boss Battles
        new ClearQuestMission() { MissionId = 10040601, QuestId = ExBossBattleQuestId },
        // Clear Six Extra Boss Battles
        new ClearQuestMission() { MissionId = 10040602, QuestId = ExBossBattleQuestId },
        // Clear 10 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10040603, QuestId = ExBossBattleQuestId },
        // Clear 15 Extra Boss Battles
        new ClearQuestMission() { MissionId = 10040604, QuestId = ExBossBattleQuestId },
        // Clear Three Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10040701 },
        // Clear Six Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10040702 },
        // Clear 10 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10040703 },
        // Clear 15 Challenge Battles
        new EventChallengeBattleClearMission() { MissionId = 10040704 },
        // Completely Clear a Challenge Battle on Expert
        new EventChallengeBattleClearMission()
        {
            MissionId = 10040801,
            FullClear = true,
            QuestId = ExpertChallengeBattleQuestId,
        },
        // Completely Clear a Challenge Battle on Master
        new EventChallengeBattleClearMission()
        {
            MissionId = 10040901,
            FullClear = true,
            QuestId = MasterChallengeBattleQuestId,
        },
        // Earn the "Shining Star" Epithet
        new EventChallengeBattleClearMission()
        {
            MissionId = 10041001,
            FullClear = true,
            QuestId = MasterChallengeBattleQuestId,
        },
    ];
}
