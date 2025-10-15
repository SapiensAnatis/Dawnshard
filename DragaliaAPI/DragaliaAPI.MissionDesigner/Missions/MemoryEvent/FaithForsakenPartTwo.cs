/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission; // For EventParticipationMission
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission, ReadQuestStoryMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class FaithForsakenPartTwo
{
    // Quest IDs from QuestNames.txt
    private const int CeaselessTragedyQuestId = 204440101;
    private const int AGlimmerOfHopeQuestId = 204440102;
    private const int AssaultOnLilithQuestId = 204440202;
    private const int SatanClashExpertQuestId = 204440302;
    private const int SatanClashNightmareQuestId = 204440501;
    private const int SatanClashOmegaQuestId = 204440601;

    private const int EpilogueStoryId = 2044409;

    // Event ID 20444 derived from QuestNames.txt

    [MissionType(MissionType.MemoryEvent)] // Using MemoryEvent as requested
    [EventId(20444)] // Event ID used directly as requested
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10170101 },
        // Clear Ceaseless Tragedy
        new ClearQuestMission() { MissionId = 10170201, QuestId = CeaselessTragedyQuestId },
        // Clear A Glimmer of Hope
        new ClearQuestMission() { MissionId = 10170301, QuestId = AGlimmerOfHopeQuestId },
        // Clear Assault on Lilith
        new ClearQuestMission() { MissionId = 10170401, QuestId = AssaultOnLilithQuestId },
        // Clear Satan Clash on Expert
        new ClearQuestMission() { MissionId = 10170501, QuestId = SatanClashExpertQuestId },
        // Clear Satan Clash on Nightmare
        new ClearQuestMission() { MissionId = 10170601, QuestId = SatanClashNightmareQuestId },
        // Read the Epilogue
        new ReadQuestStoryMission() { MissionId = 10170701, QuestStoryId = EpilogueStoryId },
        // Clear Satan Clash: Omega
        new ClearQuestMission() { MissionId = 10170801, QuestId = SatanClashOmegaQuestId },
    ];
}
