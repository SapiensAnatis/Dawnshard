using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
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
    private readonly IItemSummonService itemSummonService;
    private readonly IUpdateDataService updateDataService;

    public MypageController(
        IMissionService missionService,
        IItemSummonService itemSummonService,
        IUpdateDataService updateDataService
    )
    {
        this.missionService = missionService;
        this.itemSummonService = itemSummonService;
        this.updateDataService = updateDataService;
    }

    private static readonly List<QuestScheduleDetailList> AvailableQuestSchedule; // Used for unlocking void battles

    static MypageController()
    {
        string questScheduleJson = System.IO.File.ReadAllText(
            "Resources/mypage_info_quest_schedule.json"
        );
        AvailableQuestSchedule =
            JsonSerializer.Deserialize<List<QuestScheduleDetailList>>(questScheduleJson)
            ?? new List<QuestScheduleDetailList>();
    }

    [Route("info")]
    [HttpPost]
    public async Task<DragaliaResult> Info()
    {
        MypageInfoData resp = new();

        resp.user_summon_list = new List<UserSummonList>();
        resp.quest_event_schedule_list = new List<QuestEventScheduleList>();
        resp.quest_schedule_detail_list = AvailableQuestSchedule;
        resp.is_shop_notification =
            (await this.itemSummonService.GetOrRefreshItemSummon()).daily_summon_count == 0;
        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.update_data_list.mission_notice = await this.missionService.GetMissionNotice(null);

        return Ok(resp);
    }
}
