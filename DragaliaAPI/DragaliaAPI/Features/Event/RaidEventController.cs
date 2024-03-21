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
        RaidEventGetEventDataResponse resp = new();

        resp.IsReceiveEventDamageReward = await eventService.GetCustomEventFlag(
            request.RaidEventId
        );
        resp.RaidEventUserData = await eventService.GetRaidEventUserData(request.RaidEventId);
        resp.RaidEventRewardList = await eventService.GetEventRewardList<RaidEventRewardList>(
            request.RaidEventId
        );
        resp.EventPassiveList = new List<EventPassiveList>
        {
            await eventService.GetEventPassiveList(request.RaidEventId)
        };

        if (
            MasterAsset
                .EventTradeGroup.Enumerable.FirstOrDefault(x => x.EventId == request.RaidEventId)
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
        RaidEventEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        RaidEventEntryResponse resp = new();

        resp.RaidEventUserData = await eventService.GetRaidEventUserData(request.RaidEventId);
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("receive_raid_point_reward")]
    public async Task<DragaliaResult> ReceiveRaidPointReward(
        RaidEventReceiveRaidPointRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        RaidEventReceiveRaidPointRewardResponse resp = new();

        await eventService.ReceiveEventRewards(request.RaidEventId, request.RaidEventRewardIdList);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        resp.RaidEventRewardList = await eventService.GetEventRewardList<RaidEventRewardList>(
            request.RaidEventId
        );

        return Ok(resp);
    }
}
