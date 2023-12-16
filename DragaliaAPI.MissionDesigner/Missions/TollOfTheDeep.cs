using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using JetBrains.Annotations;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
[UsedImplicitly]
public static class TollOfTheDeep
{
    private const int EventId = 20845;

    [MissionList(Type = MissionType.MemoryEvent)]
    [UsedImplicitly]
    public static List<Mission> Missions { get; } =
        new()
        {
            // Participate In The Event
            new EventParticipationMission { MissionId = 10220101, EventId = EventId, },
            // Clear a Boss Battle
            new EventRegularBattleClearMission { MissionId = 10220201, EventId = EventId },
            // Clear a "Toll of the Deep" Quest with Having a Summer Ball Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10220401,
                EventId = EventId,
                Crest = AbilityCrests.HavingaSummerBall
            },
            // Collect 100 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220501, EventId = EventId },
            // Collect 500 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220502, EventId = EventId },
            // Collect 1,500 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220503, EventId = EventId },
            // Collect 4,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220504, EventId = EventId },
            // Collect 6,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220505, EventId = EventId },
            // Collect 7,000 Oceanic Resonance in One Go
            new EventPointCollectionRecordMission() { MissionId = 10220506, EventId = EventId },
            // Clear Five Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220601, EventId = EventId },
            // Clear 10 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220602, EventId = EventId },
            // Clear 15 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220603, EventId = EventId },
            // Clear 20 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220604, EventId = EventId },
            // Clear 30 Boss Battles
            new EventRegularBattleClearMission() { MissionId = 10220605, EventId = EventId },
            // Clear Three Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220801, EventId = EventId },
            // Clear Six Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220802, EventId = EventId },
            // Clear 10 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220803, EventId = EventId },
            // Clear 15 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220804, EventId = EventId },
            // Clear 20 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220805, EventId = EventId },
            // Clear 25 Challenge Battles
            new EventChallengeBattleClearMission() { MissionId = 10220806, EventId = EventId },
            // Completely Clear a Challenge Battle on Expert
            new EventChallengeBattleClearMission()
            {
                MissionId = 10220901,
                EventId = EventId,
                QuestId = 208450501,
                FullClear = true,
            },
            // Completely Clear a Challenge Battle on Master
            new EventChallengeBattleClearMission()
            {
                MissionId = 10221001,
                EventId = EventId,
                QuestId = 208450502,
                FullClear = true,
            },
            // Clear a "Toll of the Deep" Trial on Standard
            new EventTrialClearMission()
            {
                MissionId = 10221101,
                EventId = EventId,
                VariationType = VariationTypes.Hell,
            },
            // Clear a "Toll of the Deep" Trial on Expert
            new EventTrialClearMission()
            {
                MissionId = 10221201,
                EventId = EventId,
                VariationType = VariationTypes.Variation6,
            },
            // Earn the "Light of the Deep" Epithet
            // Earned from 'Completely Clear a Challenge Battle On Master'
            new EventChallengeBattleClearMission()
            {
                MissionId = 10221301,
                EventId = EventId,
                QuestId = 208450502,
                FullClear = true,
            },
        };
}
