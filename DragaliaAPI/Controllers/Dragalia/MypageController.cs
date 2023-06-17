using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;
using MessagePack.Resolvers;
using MessagePack;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MypageController : DragaliaControllerBase
{
    private readonly IMissionService missionService;

    public MypageController(IMissionService missionService)
    {
        this.missionService = missionService;
    }

    [Route("info")]
    [HttpPost]
    public async Task<DragaliaResult> Info()
    {
        /*byte[] blob = System.IO.File.ReadAllBytes("Resources/mypage_info");
        dynamic preset_mypage = MessagePackSerializer.Deserialize<dynamic>(
            blob,
            ContractlessStandardResolver.Options
        );

        return preset_mypage;*/

        return Ok(new MypageInfoData()
        {
            user_summon_list = new List<UserSummonList>(),
            quest_event_schedule_list = new List<QuestEventScheduleList>(),
            quest_schedule_detail_list = new List<QuestScheduleDetailList>(),
            update_data_list = new UpdateDataList()
            {
                mission_notice = await this.missionService.GetMissionNotice(null)
            }
        });
    }
}
