using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class TollOfTheDeep
{
    private const int EventId = 20845;

    [MissionList(Type = MissionType.MemoryEvent)]
    public static List<Mission> Missions { get; } =
        new()
        {
            // Participate In The Event
            new EventParticipationMission { MissionId = 10220101, EventId = EventId, },
            // Clear a Boss Battle
            new EventBossBattleMission { MissionId = 10220201, EventId = EventId },
            // Clear a "Toll of the Deep" Quest with Having a Summer Ball Equipped
            new EventQuestClearWithCrestMission()
            {
                MissionId = 10220401,
                EventId = EventId,
                Crest = AbilityCrests.HavingaSummerBall
            },
            // Collect 100 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220501, EventId = EventId },
            // Collect 500 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220502, EventId = EventId },
            // Collect 1,500 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220503, EventId = EventId },
            // Collect 4,000 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220504, EventId = EventId },
            // Collect 6,000 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220505, EventId = EventId },
            // Collect 7,000 Oceanic Resonance in One Go
            new EventPointCollectionMission() { MissionId = 10220506, EventId = EventId },
            // Clear Five Boss Battles
            new EventBossBattleMission() { MissionId = 10220601, EventId = EventId },
            // Clear 10 Boss Battles
            new EventBossBattleMission() { MissionId = 10220602, EventId = EventId },
            // Clear 15 Boss Battles
            new EventBossBattleMission() { MissionId = 10220603, EventId = EventId },
            // Clear 20 Boss Battles
            new EventBossBattleMission() { MissionId = 10220604, EventId = EventId },
            // Clear 30 Boss Battles
            new EventBossBattleMission() { MissionId = 10220605, EventId = EventId },
        };
}
