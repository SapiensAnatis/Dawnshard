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
        EarnEventGetEventDataData resp = new();

        resp.earn_event_user_data = await eventService.GetEarnEventUserData(request.event_id);
        resp.event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        if (
            MasterAsset
                .EventTradeGroup.Enumerable.FirstOrDefault(x => x.EventId == request.event_id)
                ?.Id is
            { } tradeGroupId
        )
        {
            resp.event_trade_list = tradeService.GetEventTradeList(tradeGroupId);
        }

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(EarnEventEntryRequest request)
    {
        EarnEventEntryData resp = new();

        // TODO: Complete first event mission once thats implemented
        await eventService.CreateEventData(request.event_id);

        resp.earn_event_user_data = await eventService.GetEarnEventUserData(request.event_id);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_event_point_reward")]
    public async Task<DragaliaResult> ReceiveEventPointReward(
        EarnEventReceiveEventPointRewardRequest request
    )
    {
        EarnEventReceiveEventPointRewardData resp = new();

        resp.event_reward_entity_list = await eventService.ReceiveEventRewards(request.event_id);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        return Ok(resp);
    }
}
