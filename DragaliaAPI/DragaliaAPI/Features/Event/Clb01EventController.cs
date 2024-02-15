using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("clb01_event")]
public class Clb01EventController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService
) : DragaliaControllerBase
{
    [HttpPost("get_event_data")]
    public async Task<DragaliaResult> GetEventData(Clb01EventGetEventDataRequest request)
    {
        Clb01EventGetEventDataData resp = new();

        resp.clb_01_event_user_data = await eventService.GetClb01EventUserData(request.event_id);
        resp.clb_01_event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(Clb01EventEntryRequest request)
    {
        Clb01EventEntryData resp = new();

        // TODO: Complete first event mission once thats implemented

        resp.clb_01_event_user_data = await eventService.GetClb01EventUserData(request.event_id);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_clb01_point_reward")]
    public async Task<DragaliaResult> ReceiveEventPointReward(
        Clb01EventReceiveClb01PointRewardRequest request
    )
    {
        Clb01EventReceiveClb01PointRewardData resp = new();

        resp.clb_01_event_reward_entity_list = await eventService.ReceiveEventRewards(
            request.event_id
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.clb_01_event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        return Ok(resp);
    }
}
