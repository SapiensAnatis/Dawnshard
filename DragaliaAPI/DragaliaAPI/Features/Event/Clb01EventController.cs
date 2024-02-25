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

        resp.Clb01EventUserData = await eventService.GetClb01EventUserData(request.EventId);
        resp.Clb01EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(Clb01EventEntryRequest request)
    {
        Clb01EventEntryData resp = new();

        // TODO: Complete first event mission once thats implemented

        resp.Clb01EventUserData = await eventService.GetClb01EventUserData(request.EventId);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync();
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_clb01_point_reward")]
    public async Task<DragaliaResult> ReceiveEventPointReward(
        Clb01EventReceiveClb01PointRewardRequest request
    )
    {
        Clb01EventReceiveClb01PointRewardData resp = new();

        resp.Clb01EventRewardEntityList = await eventService.ReceiveEventRewards(request.EventId);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync();
        resp.EntityResult = rewardService.GetEntityResult();

        resp.Clb01EventRewardList = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.EventId
        );

        return Ok(resp);
    }
}
