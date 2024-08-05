using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event.Summon;

[Route("event_summon")]
[ServiceFilter<EventValidationFilter>]
public class EventSummonController(EventSummonService eventSummonService) : DragaliaControllerBase
{
    [HttpPost("get_data")]
    public async Task<DragaliaResult<EventSummonGetDataResponse>> GetData(
        EventSummonGetDataRequest request
    )
    {
        return new EventSummonGetDataResponse()
        {
            BoxSummonData = await eventSummonService.GetBoxSummonData(request.EventId)
        };
    }
}
