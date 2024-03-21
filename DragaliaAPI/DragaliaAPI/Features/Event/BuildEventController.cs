using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("build_event")]
public class BuildEventController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService,
    ITradeService tradeService
) : DragaliaControllerBase
{
    [HttpPost("get_event_data")]
    public async Task<DragaliaResult> GetEventData(BuildEventGetEventDataRequest request)
    {
        BuildEventGetEventDataResponse resp = new();

        resp.IsReceivableEventDailyBonus = await eventService.GetCustomEventFlag(request.EventId);

        resp.BuildEventUserData = await eventService.GetBuildEventUserData(request.EventId);
        resp.BuildEventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        if (
            MasterAsset
                .EventTradeGroup.Enumerable.FirstOrDefault(x => x.EventId == request.EventId)
                ?.Id is
            { } tradeGroupId
        )
        {
            resp.EventTradeList = tradeService.GetEventTradeList(tradeGroupId);
        }

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(
        BuildEventEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        BuildEventEntryResponse resp = new();

        resp.IsReceivableEventDailyBonus = await eventService.GetCustomEventFlag(request.EventId);
        resp.BuildEventUserData = await eventService.GetBuildEventUserData(request.EventId);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_build_point_reward")]
    public async Task<DragaliaResult> ReceiveBuildPointReward(
        BuildEventReceiveBuildPointRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        BuildEventReceiveBuildPointRewardResponse resp = new();

        resp.BuildEventRewardEntityList = await eventService.ReceiveEventRewards(request.EventId);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        resp.BuildEventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        return Ok(resp);
    }
}
