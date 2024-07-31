using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("event_summon")]
[ServiceFilter<EventValidationFilter>]
public class EventSummonController : DragaliaControllerBase
{
    [HttpPost("get_data")]
    public async Task<DragaliaResult<EventSummonGetDataResponse>> GetData(
        EventSummonGetDataRequest request
    )
    {
        return new EventSummonGetDataResponse()
        {
            BoxSummonData = new()
            {
                EventId = 1,
                EventPoint = 2, // current points
                MaxExecCount = 3,
                RemainingQuantity = 4, // number of items left in the box?
                ResetPossible = 0, // whether the reset button shows
                BoxSummonDetail = [], // odds
                BoxSummonSeq = 2, // the box # you are on
            }
        };
    }
}
