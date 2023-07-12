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
        BuildEventGetEventDataData resp = new();

        resp.is_receivable_event_daily_bonus = await eventService.GetCustomEventFlag(
            request.event_id
        );

        resp.build_event_user_data = await eventService.GetBuildEventUserData(request.event_id);
        resp.build_event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        if (
            MasterAsset.EventTradeGroup.Enumerable
                .FirstOrDefault(x => x.EventId == request.event_id)
                ?.Id is
            { } tradeGroupId
        )
        {
            resp.event_trade_list = tradeService.GetEventTradeList(tradeGroupId);
        }

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(BuildEventEntryRequest request)
    {
        BuildEventEntryData resp = new();

        // TODO: Complete first event mission once thats implemented

        resp.is_receivable_event_daily_bonus = await eventService.GetCustomEventFlag(
            request.event_id
        );
        resp.build_event_user_data = await eventService.GetBuildEventUserData(request.event_id);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_build_point_reward")]
    public async Task<DragaliaResult> ReceiveBuildPointReward(
        BuildEventReceiveBuildPointRewardRequest request
    )
    {
        BuildEventReceiveBuildPointRewardData resp = new();

        resp.build_event_reward_entity_list = await eventService.ReceiveEventRewards(
            request.event_id
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.build_event_reward_list = await eventService.GetEventRewardList<BuildEventRewardList>(
            request.event_id
        );

        return Ok(resp);
    }
}
