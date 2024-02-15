using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class TrickOrTreasure
{
    private const int EventId = 20826;
    private const int ExBossBattleQuestId = 208260401;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10060101 },
            // Defeat the Pumpking Twice
            new EventRegularBattleClearMission() { MissionId = 10060102 },
            // Clear a "Trick or Treasure!" Quest with Plunder Pals Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10060201,
                Crest = AbilityCrests.PlunderPals
            },
            // Collect 100 Tricker Treats in One Go
            new EventPointCollectionRecordMission() { MissionId = 10060301 },
            // Collect 500 Tricker Treats in One Go
            new EventPointCollectionRecordMission() { MissionId = 10060302 },
            // Collect 1,500 Tricker Treats in One Go
            new EventPointCollectionRecordMission() { MissionId = 10060303 },
            // Collect 4,000 Tricker Treats in One Go
            new EventPointCollectionRecordMission() { MissionId = 10060304 },
            // Collect 7,000 Tricker Treats in One Go
            new EventPointCollectionRecordMission() { MissionId = 10060305 },
            // Clear a Boss Battle Five Times
            new EventRegularBattleClearMission() { MissionId = 10060401 },
            // Clear a Boss Battle 10 Times
            new EventRegularBattleClearMission() { MissionId = 10060402 },
            // Clear a Boss Battle 20 Times
            new EventRegularBattleClearMission() { MissionId = 10060403 },
            // Clear a Boss Battle 30 Times
            new EventRegularBattleClearMission() { MissionId = 10060404 },
            // Clear Three Extra Boss Battles
            new ClearQuestMission() { MissionId = 10060501, QuestId = ExBossBattleQuestId },
            // Clear Six Extra Boss Battles
            new ClearQuestMission() { MissionId = 10060502, QuestId = ExBossBattleQuestId },
            // Clear 10 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10060503, QuestId = ExBossBattleQuestId },
            // Clear 15 Extra Boss Battles
            new ClearQuestMission() { MissionId = 10060504, QuestId = ExBossBattleQuestId },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10060601, },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10060602, },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10060603, },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10060604, },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10060701,
                VariationType = VariationTypes.Normal,
                FullClear = true,
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10060801,
                VariationType = VariationTypes.Extreme,
                FullClear = true,
            },
            // Earn the "Halloween Glutton" Epithet
            // Earned from 'Completely Clear a Challenge Battle On Master'
            new EventChallengeBattleClearMission()
            {
                MissionId = 10060901,
                VariationType = VariationTypes.Extreme,
                FullClear = true,
            },
        ];
}
