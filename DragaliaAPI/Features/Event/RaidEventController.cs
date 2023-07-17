using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Trade;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("raid_event")]
public class RaidEventController(
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService,
    ITradeService tradeService
) : DragaliaControllerBase
{
    // TODO: Friendship, passive boosts, summons

    [HttpPost("get_event_data")]
    public async Task<DragaliaResult> GetEventData(RaidEventGetEventDataRequest request)
    {
        RaidEventGetEventDataData resp = new();

        resp.is_receive_event_damage_reward = await eventService.GetCustomEventFlag(
            request.raid_event_id
        );
        resp.raid_event_user_data = await eventService.GetRaidEventUserData(request.raid_event_id);
        resp.raid_event_reward_list = await eventService.GetEventRewardList<RaidEventRewardList>(
            request.raid_event_id
        );
        resp.event_passive_list = new List<EventPassiveList>
        {
            await eventService.GetEventPassiveList(request.raid_event_id)
        };

        if (
            MasterAsset.EventTradeGroup.Enumerable
                .FirstOrDefault(x => x.EventId == request.raid_event_id)
                ?.Id is
            { } tradeGroupId
        )
        {
            resp.event_trade_list = tradeService.GetEventTradeList(tradeGroupId);
        }

        return Ok(resp);
    }

    [HttpPost("entry")]
    public async Task<DragaliaResult> Entry(RaidEventEntryRequest request)
    {
        RaidEventEntryData resp = new();

        resp.raid_event_user_data = await eventService.GetRaidEventUserData(request.raid_event_id);
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_raid_point_reward")]
    public async Task<DragaliaResult> ReceiveRaidPointReward(
        RaidEventReceiveRaidPointRewardRequest request
    )
    {
        RaidEventReceiveRaidPointRewardData resp = new();

        await eventService.ReceiveEventRewards(
            request.raid_event_id,
            request.raid_event_reward_id_list
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        resp.raid_event_reward_list = await eventService.GetEventRewardList<RaidEventRewardList>(
            request.raid_event_id
        );

        return Ok(resp);
    }
}
