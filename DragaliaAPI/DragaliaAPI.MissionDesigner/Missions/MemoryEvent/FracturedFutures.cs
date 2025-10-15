/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission and ReadQuestStoryMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class FracturedFutures
{
    // Event ID 20427 derived from QuestNames.txt/StorySectionNames.txt

    // Quest IDs from QuestNames.txt
    private const int ShiningCyclopsAssaultQuestId = 204270202;
    private const int ChronosClashQuestId = 204270302;
    private const int ChronosNyxClashQuestId = 204270501;
    private const int ChronosClashOmegaQuestId = 204270601;
    private const int ChronosNyxClashOmegaQuestId = 204270602;
    private const int TheDecisiveBattleQuestId = 204270603;
    private const int TimesEndQuestId = 204270604;

    // Story IDs from StorySectionNames.txt
    private const int EpilogueStoryId = 2042707;

    [MissionType(MissionType.MemoryEvent)]
    [EventId(20427)] // Event ID used directly as requested
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10090101 },
        // Clear Shining Cyclops Assault
        new ClearQuestMission() { MissionId = 10090201, QuestId = ShiningCyclopsAssaultQuestId },
        // Clear Chronos Clash
        new ClearQuestMission() { MissionId = 10090301, QuestId = ChronosClashQuestId },
        // Clear Chronos Nyx Clash
        new ClearQuestMission() { MissionId = 10090401, QuestId = ChronosNyxClashQuestId },
        // Read the Epilogue
        new ReadQuestStoryMission() { MissionId = 10090501, QuestStoryId = EpilogueStoryId },
        // Clear Chronos Clash: Omega
        new ClearQuestMission() { MissionId = 10090601, QuestId = ChronosClashOmegaQuestId },
        // Clear Chronos Nyx Clash: Omega
        new ClearQuestMission() { MissionId = 10090701, QuestId = ChronosNyxClashOmegaQuestId },
        // Clear The Decisive Battle
        new ClearQuestMission() { MissionId = 10090801, QuestId = TheDecisiveBattleQuestId },
        // Clear Time's End
        new ClearQuestMission() { MissionId = 10090901, QuestId = TimesEndQuestId },
    ];
}
