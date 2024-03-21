using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("combat_event")]
public class CombatEventController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService,
    ITradeService tradeService
) : DragaliaControllerBase
{
    [HttpPost("get_event_data")]
    public async Task<DragaliaResult> GetEventData(CombatEventGetEventDataRequest request)
    {
        CombatEventGetEventDataResponse resp = new();

        resp.CombatEventUserData = await eventService.GetCombatEventUserData(request.EventId);
        resp.EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );
        resp.UserEventLocationRewardList =
            await eventService.GetEventRewardList<UserEventLocationRewardList>(request.EventId);

        resp.EventTradeList = MasterAsset
            .EventTradeGroup.Enumerable.Where(x => x.EventId == request.EventId)
            .SelectMany(x => tradeService.GetEventTradeList(x.Id));

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(
        CombatEventEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventEntryResponse resp = new();

        // TODO: Complete first event mission once thats implemented

        resp.CombatEventUserData = await eventService.GetCombatEventUserData(request.EventId);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_event_point_reward")]
    public async Task<DragaliaResult> ReceiveCombatPointReward(
        CombatEventReceiveEventPointRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventReceiveEventPointRewardResponse resp = new();

        resp.EventRewardEntityList = await eventService.ReceiveEventRewards(request.EventId);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        resp.EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        return Ok(resp);
    }

    [HttpPost("receive_event_location_reward")]
    public async Task<DragaliaResult> ReceiveEventLocationReward(
        CombatEventReceiveEventLocationRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventReceiveEventLocationRewardResponse resp = new();

        resp.EventLocationRewardEntityList = await eventService.ReceiveEventLocationReward(
            request.EventId,
            request.EventLocationRewardId
        );

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        resp.UserEventLocationRewardList =
            await eventService.GetEventRewardList<UserEventLocationRewardList>(request.EventId);

        return Ok(resp);
    }
}
