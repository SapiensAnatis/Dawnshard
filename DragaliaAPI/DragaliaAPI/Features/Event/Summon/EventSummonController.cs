using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event.Summon;

[Route("event_summon")]
[ServiceFilter<EventValidationFilter>]
public class EventSummonController(
    EventSummonService eventSummonService,
    IRewardService rewardService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("get_data")]
    public async Task<DragaliaResult<EventSummonGetDataResponse>> GetData(
        EventSummonGetDataRequest request,
        CancellationToken cancellationToken
    )
    {
        return new EventSummonGetDataResponse()
        {
            BoxSummonData = await eventSummonService.GetBoxSummonData(
                request.EventId,
                cancellationToken
            )
        };
    }

    [HttpPost("exec")]
    public async Task<DragaliaResult<EventSummonExecResponse>> Exec(
        EventSummonExecRequest request,
        CancellationToken cancellationToken
    )
    {
        AtgenBoxSummonResult boxResult = await eventSummonService.ExecuteBoxSummon(
            request.EventId,
            request.Count,
            request.IsEnableStopByTarget
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        EntityResult entityResult = rewardService.GetEntityResult();

        return new EventSummonExecResponse()
        {
            BoxSummonResult = boxResult,
            EntityResult = entityResult,
            UpdateDataList = updateDataList,
        };
    }
}
