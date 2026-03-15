using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
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
        EarnEventGetEventDataResponse resp = new()
        {
            EarnEventUserData = await eventService.GetEarnEventUserData(request.EventId),
            EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
                request.EventId
            ),
        };

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
        EarnEventReceiveEventPointRewardResponse resp = new()
        {
            EventRewardEntityList = await eventService.ReceiveEventRewards(request.EventId),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
            EntityResult = rewardService.GetEntityResult(),
            EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
                request.EventId
            ),
        };

        return Ok(resp);
    }
}
