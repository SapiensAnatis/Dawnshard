using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mission")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MissionController : ControllerBase
{
    private static readonly List<DrillMission> drillMissionList = new List<DrillMission>();

    public MissionController()
    {
        for (int i = 0; i <= 54; i++)
        {
            int id = 100000 + (100 * i);
            drillMissionList.Add(new(id, 0, 0, 0, 0));
        }
    }

    [HttpPost]
    [Route("get_mission_list")]
    public DragaliaResult GetMissionList()
    {
        return Ok(new GetMissionListResponse(GetMissionListFactory.CreateData()));
    }

    [HttpPost]
    [Route("get_drill_mission_list")]
    public DragaliaResult GetDrillMissionList()
    {
        GetDrillMissionListData data =
            new(drillMissionList, GetMissionListFactory.emptyMissionNoticeData);
        return Ok(new GetDrillMissionListResponse(data));
    }

    [HttpPost]
    [Route("unlock_drill_mission_group")]
    public DragaliaResult UnlockDrillMissionGroup()
    {
        UnlockDrillMissionGroupData data =
            new(
                drillMissionList,
                new
                {
                    mission_notice = GetMissionListFactory.emptyMissionNoticeData,
                    current_main_story_mission = new List<object>(),
                    functional_maintenance_list = new List<object>()
                }
            );

        return Ok(new UnlockDrillMissionGroupResponse(data));
    }
}
