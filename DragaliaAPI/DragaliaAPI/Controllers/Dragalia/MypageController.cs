using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MypageController(
    IMissionService missionService,
    IShopRepository shopRepository,
    IAutoRepeatService autoRepeatService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [Route("info")]
    [HttpPost]
    public async Task<DragaliaResult> Info(CancellationToken cancellationToken)
    {
        MypageInfoResponse resp = new();

        resp.UserSummonList = new List<UserSummonList>();
        resp.QuestEventScheduleList = new List<QuestEventScheduleList>();

        resp.QuestScheduleDetailList = MasterAsset.QuestScheduleInfo.Enumerable.Select(
            x => new QuestScheduleDetailList(
                x.Id,
                x.ScheduleGroupId,
                x.DropBonusCount,
                0,
                x.IntervalType,
                x.StartDate,
                x.EndDate
            )
        );

        resp.IsShopNotification = await shopRepository.GetDailySummonCountAsync() == 0;
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.UpdateDataList.MissionNotice = await missionService.GetMissionNotice(null);

        RepeatInfo? repeatInfo = await autoRepeatService.GetRepeatInfo();

        if (repeatInfo != null)
            resp.RepeatData = new(repeatInfo.Key.ToString(), repeatInfo.CurrentCount, 1);

        return Ok(resp);
    }
}
