using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mission")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MissionController : DragaliaControllerBase
{
    private static class Data
    {
        private static readonly List<NormalMissionList> normalMissions =
            new() { new(10000101, 0, 0, 0, 0) };
        private static readonly List<DailyMissionList> dailyMissions =
            new() { new(15070101, 0, 0, 0, 0, 0, int.MaxValue, 0, 0, 0) };
        private static readonly List<PeriodMissionList> periodMissions =
            new() { new(12050101, 0, 0, 0, int.MaxValue) };
        private static readonly List<SpecialMissionList> specialMissions =
            new() { new(10000101, 0, 0, 0, 0) };
        private static readonly List<AlbumMissionList> albumMissions =
            new() { new(10010101, 0, 0, 0, 0) };
        private static readonly AtgenNormalMissionNotice emptyMissionNotice =
            new(false, 0, 0, 0, 0, 0, new List<int>());

        public static readonly MissionNotice EmptyMissionNoticeData =
            new(
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice,
                emptyMissionNotice
            );

        public static MissionGetMissionListData MissionList =
            new(
                normal_mission_list: normalMissions,
                daily_mission_list: dailyMissions,
                period_mission_list: periodMissions,
                beginner_mission_list: new List<BeginnerMissionList>(),
                special_mission_list: specialMissions,
                special_mission_purchased_group_id_list: new List<int>(),
                main_story_mission_list: new List<MainStoryMissionList>(),
                current_main_story_mission: new CurrentMainStoryMission(
                    0,
                    new List<AtgenMainStoryMissionStateList>()
                ),
                memory_event_mission_list: new List<MemoryEventMissionList>(),
                album_mission_list: albumMissions,
                mission_notice: EmptyMissionNoticeData
            );

        public static readonly List<DrillMissionList> DrillMissions = new();
    }

    public MissionController()
    {
        for (int i = 0; i <= 54; i++)
        {
            int id = 100000 + (100 * i);
            Data.DrillMissions.Add(new(id, 0, 0, 0, 0));
        }
    }

    [HttpPost]
    [Route("get_mission_list")]
    public DragaliaResult GetMissionList()
    {
        return Ok(Data.MissionList);
    }

    [HttpPost]
    [Route("get_drill_mission_list")]
    public DragaliaResult GetDrillMissionList()
    {
        return Ok(
            new MissionGetDrillMissionListData(
                Data.DrillMissions,
                new List<DrillMissionGroupList>(),
                Data.EmptyMissionNoticeData
            )
        );
    }

    [HttpPost]
    [Route("unlock_drill_mission_group")]
    public DragaliaResult UnlockDrillMissionGroup()
    {
        return Ok(
            new MissionUnlockDrillMissionGroupData(
                Data.DrillMissions,
                // TODO track missions in DB so we can get update data from there
                new()
                {
                    mission_notice = Data.EmptyMissionNoticeData,
                    current_main_story_mission = null,
                    functional_maintenance_list = null,
                },
                new()
            )
        );
    }
}
