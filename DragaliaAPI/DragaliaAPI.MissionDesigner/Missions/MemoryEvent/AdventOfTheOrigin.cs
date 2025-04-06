/* This file was AI-generated */

using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission; // For EventParticipationMission
using DragaliaAPI.MissionDesigner.Models.RegularMission; // For ClearQuestMission, ReadQuestStoryMission

namespace DragaliaAPI.MissionDesigner.Missions.MemoryEvent;

[ContainsMissionList]
public static class AdventOfTheOrigin
{
    // Quest IDs from QuestNames.txt
    private const int VolcanicCyclopsAssaultQuestId = 204620201;
    private const int TrueBahamutClashExpertQuestId = 204620302;
    private const int TrueBahamutClashNightmareQuestId = 204620501;
    private const int TrueBahamutClashOmegaQuestId = 204620601;

    // Story IDs from StorySectionNames.txt
    private const int EpilogueStoryId = 2046208;

    // Event ID 20462 derived from QuestNames.txt

    [MissionType(MissionType.MemoryEvent)] // Using MemoryEvent as requested
    [EventId(20462)] // Event ID used directly as requested
    public static List<Mission> Missions { get; } =
        [
            // Participate in the Event
            new EventParticipationMission() { MissionId = 10280101 },
            // Clear Volcanic Cyclops Assault
            new ClearQuestMission()
            {
                MissionId = 10280201,
                QuestId = VolcanicCyclopsAssaultQuestId,
            },
            // Clear True Bahamut Clash on Expert
            new ClearQuestMission()
            {
                MissionId = 10280301,
                QuestId = TrueBahamutClashExpertQuestId,
            },
            // Clear True Bahamut Clash on Nightmare
            new ClearQuestMission()
            {
                MissionId = 10280401,
                QuestId = TrueBahamutClashNightmareQuestId,
            },
            // Read the Epilogue
            new ReadQuestStoryMission() { MissionId = 10280501, QuestStoryId = EpilogueStoryId },
            // Read the Epilogue
            // Note: Duplicate Mission ID and task as 10280501, generated as requested.
            new ReadQuestStoryMission() { MissionId = 10280601, QuestStoryId = EpilogueStoryId },
            // Read the Epilogue
            // Note: Duplicate Mission ID and task as 10280501, generated as requested.
            new ReadQuestStoryMission() { MissionId = 10280701, QuestStoryId = EpilogueStoryId },
            // Read the Epilogue
            // Note: Duplicate Mission ID and task as 10280501, generated as requested.
            new ReadQuestStoryMission() { MissionId = 10280801, QuestStoryId = EpilogueStoryId },
            // Read the Epilogue
            // Note: Duplicate Mission ID and task as 10280501, generated as requested.
            new ReadQuestStoryMission() { MissionId = 10280901, QuestStoryId = EpilogueStoryId },
            // Clear True Bahamut Clash: Omega
            new ClearQuestMission()
            {
                MissionId = 10281001,
                QuestId = TrueBahamutClashOmegaQuestId,
            },
        ];
}
