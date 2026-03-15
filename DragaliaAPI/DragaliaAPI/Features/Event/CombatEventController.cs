using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
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
        CombatEventGetEventDataResponse resp = new()
        {
            CombatEventUserData = await eventService.GetCombatEventUserData(request.EventId),
            EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
                request.EventId
            ),
            UserEventLocationRewardList =
                await eventService.GetEventRewardList<UserEventLocationRewardList>(request.EventId),
            EventTradeList = MasterAsset
                .EventTradeGroup.Enumerable.Where(x => x.EventId == request.EventId)
                .SelectMany(x => tradeService.GetEventTradeList(x.Id)),
        };

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(
        CombatEventEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventEntryResponse resp = new()
        {
            // TODO: Complete first event mission once thats implemented
            CombatEventUserData = await eventService.GetCombatEventUserData(request.EventId),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
            EntityResult = rewardService.GetEntityResult(),
        };

        return Ok(resp);
    }

    [HttpPost("receive_event_point_reward")]
    public async Task<DragaliaResult> ReceiveCombatPointReward(
        CombatEventReceiveEventPointRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventReceiveEventPointRewardResponse resp = new()
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

    [HttpPost("receive_event_location_reward")]
    public async Task<DragaliaResult> ReceiveEventLocationReward(
        CombatEventReceiveEventLocationRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        CombatEventReceiveEventLocationRewardResponse resp = new()
        {
            EventLocationRewardEntityList = await eventService.ReceiveEventLocationReward(
                request.EventId,
                request.EventLocationRewardId
            ),
            UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken),
            EntityResult = rewardService.GetEntityResult(),
            UserEventLocationRewardList =
                await eventService.GetEventRewardList<UserEventLocationRewardList>(request.EventId),
        };

        return Ok(resp);
    }
}
