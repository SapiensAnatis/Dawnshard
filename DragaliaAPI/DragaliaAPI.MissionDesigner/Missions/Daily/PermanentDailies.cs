using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions;

[ContainsMissionList]
public static class PermanentDailies
{
    private const int ProgressionGroupId = 15070;

    [MissionType(MissionType.Daily)]
    public static List<Mission> Missions { get; } =
        [
            // Perform an Item Summon
            new ItemSummonMission()
            {
                MissionId = 15070101,
                ProgressionGroupId = ProgressionGroupId
            },
            // Collect Rupies from a Facility
            new FortIncomeCollectedMission()
            {
                MissionId = 15070201,
                EntityType = EntityTypes.Rupies,
                ProgressionGroupId = ProgressionGroupId
            },
            // Clear a Quest
            new ClearQuestMission()
            {
                MissionId = 15070301,
                ProgressionGroupId = ProgressionGroupId
            },
            // Clear Three Quests
            new ClearQuestMission()
            {
                MissionId = 15070401,
                ProgressionGroupId = ProgressionGroupId
            },
            // Clear Five Quests
            new ClearQuestMission()
            {
                MissionId = 15070501,
                ProgressionGroupId = ProgressionGroupId
            },
            // Clear All Standard Daily Endeavors
            new ClearProgressionGroupMission()
            {
                MissionId = 15070601,
                ProgressionGroupToClear = ProgressionGroupId
            }
        ];
}
