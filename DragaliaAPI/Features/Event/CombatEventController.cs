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
        CombatEventGetEventDataData resp = new();

        resp.combat_event_user_data = await eventService.GetCombatEventUserData(request.event_id);
        resp.event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );
        resp.user_event_location_reward_list =
            await eventService.GetEventRewardList<UserEventLocationRewardList>(request.event_id);

        resp.event_trade_list = MasterAsset.EventTradeGroup.Enumerable
            .Where(x => x.EventId == request.event_id)
            .SelectMany(x => tradeService.GetEventTradeList(x.Id));

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(CombatEventEntryRequest request)
    {
        CombatEventEntryData resp = new();

        // TODO: Complete first event mission once thats implemented

        resp.combat_event_user_data = await eventService.GetCombatEventUserData(request.event_id);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_event_point_reward")]
    public async Task<DragaliaResult> ReceiveCombatPointReward(
        CombatEventReceiveEventPointRewardRequest request
    )
    {
        CombatEventReceiveEventPointRewardData resp = new();

        resp.event_reward_entity_list = await eventService.ReceiveEventRewards(request.event_id);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        return Ok(resp);
    }

    [HttpPost("receive_event_location_reward")]
    public async Task<DragaliaResult> ReceiveEventLocationReward(
        CombatEventReceiveEventLocationRewardRequest request
    )
    {
        CombatEventReceiveEventLocationRewardData resp = new();

        resp.event_location_reward_entity_list = await eventService.ReceiveEventLocationReward(
            request.event_id,
            request.event_location_reward_id
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.user_event_location_reward_list =
            await eventService.GetEventRewardList<UserEventLocationRewardList>(request.event_id);

        return Ok(resp);
    }
}
