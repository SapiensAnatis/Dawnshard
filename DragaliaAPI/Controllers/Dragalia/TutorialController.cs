using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MessagePack;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class TutorialController : ControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly ISessionService _sessionService;

    public TutorialController(
        IUserDataRepository userDataRepository,
        ISessionService sessionService
    )
    {
        this.userDataRepository = userDataRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    [Route("update_step")]
    public async Task<DragaliaResult> UpdateStep(
        [FromHeader(Name = "SID")] string sessionId,
        TutorialUpdateStepRequest request
    )
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await userDataRepository.UpdateTutorialStatus(
            deviceAccountId,
            request.step
        );

        UpdateDataList updateDataList =
            new() { user_data = SavefileUserDataFactory.Create(userData) };
        TutorialUpdateStepResponse response =
            new(new TutorialUpdateStepData(request.step, updateDataList));

        return Ok(response);
    }

    [MessagePackObject(true)]
    public record UpdateFlagsRequest(int flag_id);

    [HttpPost]
    [Route("update_flags")]
    public async Task<DragaliaResult> UpdateFlags(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] UpdateFlagsRequest flagRequest
    )
    {
        int flag_id = flagRequest.flag_id;
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbPlayerUserData userData = await this.userDataRepository.AddTutorialFlag(
            deviceAccountId,
            flag_id
        );

        UpdateDataList updateDataList =
            new() { user_data = SavefileUserDataFactory.Create(userData) };
        TutorialUpdateFlagsResponse response =
            new(
                new TutorialUpdateFlagsData(
                    new() { flag_id },
                    updateDataList,
                    new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                )
            );

        return Ok(response);
    }
}
