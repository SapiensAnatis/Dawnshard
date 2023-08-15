using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;
using MessagePack.Resolvers;
using MessagePack;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MypageController(
    IMissionService missionService,
    IShopRepository shopRepository,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [Route("info")]
    [HttpPost]
    public async Task<DragaliaResult> Info()
    {
        MypageInfoData resp = new();

        resp.user_summon_list = new List<UserSummonList>();
        resp.quest_event_schedule_list = new List<QuestEventScheduleList>();
        resp.quest_schedule_detail_list = MasterAsset.QuestScheduleInfo.Enumerable.Select(
            x =>
                new QuestScheduleDetailList(
                    x.Id,
                    x.ScheduleGroupId,
                    x.DropBonusCount,
                    0,
                    x.IntervalType,
                    x.StartDate,
                    x.EndDate
                )
        );
        resp.is_shop_notification = await shopRepository.GetDailySummonCountAsync() == 0;
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.update_data_list.mission_notice = await missionService.GetMissionNotice(null);

        return Ok(resp);
    }
}
