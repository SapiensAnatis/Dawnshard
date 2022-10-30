using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("/tutorial")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class TutorialController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public TutorialController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
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
        DbPlayerUserData userData = await _apiRepository.UpdateTutorialStatus(
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
        DbPlayerUserData userData = await _apiRepository.UpdateTutorialFlag(
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
