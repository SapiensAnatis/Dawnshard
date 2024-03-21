using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class TollOfTheDeep
{
    private const int EventId = 20845;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
        new()
        {
            // Participate In The Event
            new EventParticipationMission { MissionId = 10220101, },
            // Clear a Boss Battle
            new EventRegularBattleClearMission { MissionId = 10220201 },
            // Clear a "Toll of the Deep" Quest with Having a Summer Ball Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10220401,
                Crest = AbilityCrests.HavingaSummerBall
            },
            // Collect 100 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220501 },
            // Collect 500 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220502 },
            // Collect 1,500 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220503 },
            // Collect 4,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220504 },
            // Collect 6,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220505 },
            // Collect 7,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220506 },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220601 },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220602 },
            // Clear 15 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220603 },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220604 },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220605 },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220801 },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220802 },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220803 },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220804 },
            // Clear 20 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220805 },
            // Clear 25 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220806 },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10220901,
                VariationType = VariationTypes.VeryHard,
                FullClear = true,
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10221001,
                VariationType = VariationTypes.Extreme,
                FullClear = true,
            },
            // Clear a "Toll of the Deep" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 10221101,
                VariationType = VariationTypes.Hell,
            },
            // Clear a "Toll of the Deep" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 10221201,
                VariationType = VariationTypes.Variation6,
            },
            // Earn the "Light of the Deep" Epithet
            // Earned from 'Completely Clear a Challenge Battle On Master'
            new EventChallengeBattleClearMission()
            {
                MissionId = 10221301,
                VariationType = VariationTypes.Extreme,
                FullClear = true,
            },
        };
}
