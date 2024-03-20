using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Event;

[Route("memory_event")]
public class MemoryEventController(
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IRewardService rewardService,
    IEventService eventService
) : DragaliaControllerBase
{
    [HttpPost("activate")]
    public async Task<DragaliaResult> Activate(
        MemoryEventActivateRequest request,
        CancellationToken cancellationToken
    )
    {
        MemoryEventActivateResponse resp = new();

        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();

        await eventService.CreateEventData(request.EventId);
        userData.ActiveMemoryEventId = request.EventId;

        resp.Result = 1;
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }
}
