using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("earn_event")]
public class EarnEventController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService,
    ITradeService tradeService
) : DragaliaControllerBase
{
    // TODO: This is not fully implemented. However, no compendium event uses it.

    [HttpPost("get_event_data")]
    public async Task<DragaliaResult> GetEventData(EarnEventGetEventDataRequest request)
    {
        EarnEventGetEventDataResponse resp = new();

        resp.EarnEventUserData = await eventService.GetEarnEventUserData(request.EventId);
        resp.EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
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
        EarnEventEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        EarnEventEntryResponse resp = new();

        // TODO: Complete first event mission once thats implemented
        await eventService.CreateEventData(request.EventId);

        resp.EarnEventUserData = await eventService.GetEarnEventUserData(request.EventId);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_event_point_reward")]
    public async Task<DragaliaResult> ReceiveEventPointReward(
        EarnEventReceiveEventPointRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        EarnEventReceiveEventPointRewardResponse resp = new();

        resp.EventRewardEntityList = await eventService.ReceiveEventRewards(request.EventId);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        resp.EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        return Ok(resp);
    }
}
