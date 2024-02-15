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
    public async Task<DragaliaResult> Activate(MemoryEventActivateRequest request)
    {
        MemoryEventActivateData resp = new();

        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();

        await eventService.CreateEventData(request.event_id);
        userData.ActiveMemoryEventId = request.event_id;

        resp.result = 1;
        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }
}
