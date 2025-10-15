/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission; // For EventParticipationMission
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission, ReadQuestStoryMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class FaithForsakenPartOne
{
    // Quest IDs from QuestNames.txt
    private const int ResistanceOfTheInnocentQuestId = 204430101;
    private const int TheMightOfHeavenQuestId = 204430102;
    private const int AssaultOnMichaelQuestId = 204430202;
    private const int AsuraClashExpertQuestId = 204430302;
    private const int AsuraClashNightmareQuestId = 204430501;
    private const int AsuraClashOmegaQuestId = 204430601;

    // Story IDs from StorySectionNames.txt
    private const int EpilogueStoryId = 2044309;

    // Event ID 20443 derived from QuestNames.txt

    [MissionType(MissionType.MemoryEvent)] // Using MemoryEvent as requested
    [EventId(20443)] // Event ID used directly as requested
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 10160101 },
        // Clear Resistance of the Innocent
        new ClearQuestMission() { MissionId = 10160201, QuestId = ResistanceOfTheInnocentQuestId },
        // Clear The Might of Heaven
        new ClearQuestMission() { MissionId = 10160301, QuestId = TheMightOfHeavenQuestId },
        // Clear Assault on Michael
        new ClearQuestMission() { MissionId = 10160401, QuestId = AssaultOnMichaelQuestId },
        // Clear Asura Clash on Expert
        new ClearQuestMission() { MissionId = 10160501, QuestId = AsuraClashExpertQuestId },
        // Clear Asura Clash on Nightmare
        new ClearQuestMission() { MissionId = 10160601, QuestId = AsuraClashNightmareQuestId },
        // Read the Epilogue
        new ReadQuestStoryMission() { MissionId = 10160701, QuestStoryId = EpilogueStoryId },
        // Clear Asura Clash: Omega
        new ClearQuestMission() { MissionId = 10160801, QuestId = AsuraClashOmegaQuestId },
    ];
}
