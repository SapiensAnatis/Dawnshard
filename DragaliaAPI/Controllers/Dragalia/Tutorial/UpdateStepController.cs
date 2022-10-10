using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia.Tutorial;

[Route("/tutorial/update_step")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UpdateStepController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public UpdateStepController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(
        [FromHeader(Name = "SID")] string sessionId,
        TutorialUpdateStepRequest request
    )
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbSavefileUserData userData = await _apiRepository.UpdateTutorialStatus(
            deviceAccountId,
            request.step
        );

        UpdateDataList updateDataList = new(SavefileUserDataFactory.Create(userData, new()));
        TutorialUpdateStepResponse response =
            new(new TutorialUpdateStepData(request.step, updateDataList));

        return Ok(response);
    }
}
