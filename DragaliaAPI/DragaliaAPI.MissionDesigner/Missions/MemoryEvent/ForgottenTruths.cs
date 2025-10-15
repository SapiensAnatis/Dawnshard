/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission; // For EventParticipationMission
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission, ReadQuestStoryMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent; // Namespace adjusted for Raid Event

[ContainsMissionList]
public static class ForgottenTruths
{
    // Quest IDs from QuestNames.txt
    private const int AssaultOnZephyrId = 204280202;
    private const int MorsayatiClashExpertId = 204280302;
    private const int MorsayatiClashNightmareId = 204280501;
    private const int MorsayatiClashOmegaId = 204280601;
    private const int WarOfBindingClimaxId = 204280602;

    // Story IDs from StorySectionNames.txt
    private const int EpilogueStoryId = 2042811;

    // Event ID 20428 derived from QuestNames.txt

    [MissionType(MissionType.MemoryEvent)] // Identified as Raid Event
    [EventId(20428)] // Event ID used directly as requested
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10080101 },
        // Clear Assault on Zephyr
        new ClearQuestMission() { MissionId = 10080201, QuestId = AssaultOnZephyrId },
        // Clear Morsayati Clash on Expert
        new ClearQuestMission() { MissionId = 10080301, QuestId = MorsayatiClashExpertId },
        // Clear Morsayati Clash on Nightmare
        new ClearQuestMission() { MissionId = 10080401, QuestId = MorsayatiClashNightmareId },
        // Read the Epilogue
        new ReadQuestStoryMission() { MissionId = 10080501, QuestStoryId = EpilogueStoryId },
        // Clear Morsayati Clash: Omega
        new ClearQuestMission() { MissionId = 10080601, QuestId = MorsayatiClashOmegaId },
        // Clear The War of Binding's Climax
        new ClearQuestMission() { MissionId = 10080701, QuestId = WarOfBindingClimaxId },
    ];
}
