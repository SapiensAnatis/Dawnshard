using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class AccursedArchives
{
    private const int EventId = 20831;
    private const int ExBossBattleQuestId = 208310401;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10100101 },
            // Clear a Boss Battle
            new EventRegularBattleClearMission() { MissionId = 10100201 },
            // Clear a "The Accursed Archives" Quest with Hitting the Books Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10100401,
                Crest = AbilityCrests.HittingtheBooks
            },
            // Collect 100 Forbidden Knowledge in One Go
            new EventPointCollectionRecordMission() { MissionId = 10100501 },
            // Collect 500 Forbidden Knowledge in One Go
            new EventPointCollectionRecordMission() { MissionId = 10100502 },
            // Collect 1,500 Forbidden Knowledge in One Go
            new EventPointCollectionRecordMission() { MissionId = 10100503 },
            // Collect 4,000 Forbidden Knowledge in One Go
            new EventPointCollectionRecordMission() { MissionId = 10100504 },
            // Collect 7,000 Forbidden Knowledge in One Go
            new EventPointCollectionRecordMission() { MissionId = 10100505 },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10100601 },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10100602 },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10100603 },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10100604 },
            // Clear Three Extra Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10100701 },
            // Clear Six Extra Boss Battles
            new ClearQuestMission() { MissionId = 10100702, QuestId = ExBossBattleQuestId },
            // Clear 10 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10100703, QuestId = ExBossBattleQuestId },
            // Clear 15 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10100704, QuestId = ExBossBattleQuestId },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10100801 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10100802 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10100803 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10100804 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10100901,
                FullClear = true,
                QuestId = 208310501,
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10101001,
                FullClear = true,
                QuestId = 208310502,
            },
            // Earn the "Knower of the Absolute" Epithet
            new EventChallengeBattleClearMission()
            {
                MissionId = 10101101,
                FullClear = true,
                QuestId = 208310502,
            },
        ];
}
