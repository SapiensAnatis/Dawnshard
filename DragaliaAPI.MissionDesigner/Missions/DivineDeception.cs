using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class DivineDeception
{
    private const int EventId = 20844;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10210101 },
            // Clear a Boss Battle
            new EventRegularBattleClearMission() { MissionId = 10210201 },
            // Clear a "Divine Deception" Quest with Extreme Teamwork Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10210401,
                Crest = AbilityCrests.ExtremeTeamwork
            },
            // Collect 100 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210501 },
            // Collect 500 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210502 },
            // Collect 1,500 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210503 },
            // Collect 4,000 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210504 },
            // Collect 6,000 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210505 },
            // Collect 7,000 Intelligence in One Go
            new EventPointCollectionRecordMission() { MissionId = 10210506 },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10210601 },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10210602 },
            // Clear 15 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10210603 },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10210604 },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10210605 },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210801 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210802 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210803 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210804 },
            // Clear 20 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210805 },
            // Clear 25 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10210806 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10210901,
                FullClear = true,
                VariationType = VariationTypes.VeryHard
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10211001,
                FullClear = true,
                VariationType = VariationTypes.Extreme
            },
            // Clear a "Divine Deception" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 10211101,
                VariationType = VariationTypes.Hell
            },
            // Clear a "Divine Deception" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 10211201,
                VariationType = VariationTypes.Variation6
            },
            // Earn the "Fate Fighter" Epithet
            // Earned from "Completely Clear a Challenge Battle on Master"
            new EventChallengeBattleClearMission()
            {
                MissionId = 10211301,
                FullClear = true,
                VariationType = VariationTypes.Extreme
            },
        ];
}
