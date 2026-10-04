using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.Daily;

[ContainsMissionList]
public static class CagedDesireDaily
{
    private const int EventId = 20429;

    [MissionType(MissionType.Daily)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
    [
        // Clear Three "Caged Desire" Quests
        new ClearQuestMission() { MissionId = 10830101, QuestGroupId = EventId },
        // Clear a Raid Battle
        new ClearRaidMission() { MissionId = 10830201 },
        // Clear Warden Assault
        new EventRegularBattleClearMission() { MissionId = 10830301 },
        // Collect 10 Blazons
        new EventPointCollectionMission() { MissionId = 10830401 },
        // Collect 50 Blazons
        new EventPointCollectionMission() { MissionId = 10830402 },
        // Collect 100 Blazons
        new EventPointCollectionMission() { MissionId = 10830403 },
        // Collect 200 Blazons
        new EventPointCollectionMission() { MissionId = 10830404 },
        // Collect 300 Blazons
        new EventPointCollectionMission() { MissionId = 10830405 },
        // The real event rotated which days it ran missions on, so there are 5 missions for each task - we give them
        // all each day, so only implement one of them to avoid getting 5 missions for the same task every day
        // Clear Joker's Trial: Expert (Co-op)
        new ClearQuestMission() { MissionId = 10830503, QuestId = 204290805 },
        // new ClearQuestMission() { MissionId = 10830504, QuestId = 204290805 },
        // new ClearQuestMission() { MissionId = 10830505, QuestId = 204290805 },
        // new ClearQuestMission() { MissionId = 10830506, QuestId = 204290805 },
        // new ClearQuestMission() { MissionId = 10830507, QuestId = 204290805 },

        // Clear Joker's Trial: Master (Co-op)
        new ClearQuestMission() { MissionId = 10830601, QuestId = 204290806 },
        // new ClearQuestMission() { MissionId = 10830602, QuestId = 204290806 },
        // new ClearQuestMission() { MissionId = 10830603, QuestId = 204290806 },
        // new ClearQuestMission() { MissionId = 10830604, QuestId = 204290806 },

        // Clear Sophie's Trial: Expert (Co-op)
        new ClearQuestMission() { MissionId = 10830703, QuestId = 204290807 },
        // new ClearQuestMission() { MissionId = 10830704, QuestId = 204290807 },
        // new ClearQuestMission() { MissionId = 10830705, QuestId = 204290807 },
        // new ClearQuestMission() { MissionId = 10830706, QuestId = 204290807 },
        // new ClearQuestMission() { MissionId = 10830707, QuestId = 204290807 },

        // Clear Sophie's Trial: Master (Co-op)
        new ClearQuestMission() { MissionId = 10830801, QuestId = 204290808 },
        // new ClearQuestMission() { MissionId = 10830802, QuestId = 204290808 },
        // new ClearQuestMission() { MissionId = 10830803, QuestId = 204290808 },
        // new ClearQuestMission() { MissionId = 10830804, QuestId = 204290808 },
    ];
}
