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
    private readonly ApiContext _apiContext;
    private readonly ISessionService _sessionService;

    public UpdateStepController(ApiContext apiContext, ISessionService sessionService)
    {
        _apiContext = apiContext;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(TutorialUpdateStepRequest request)
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(
            Request.Headers["SID"]
        );

        DbSavefileUserData userData =
            await _apiContext.SavefileUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
        userData.TutorialStatus = request.step;
        await _apiContext.SaveChangesAsync();

        UpdateDataList updateDataList = new(SavefileUserDataFactory.Create(userData, new()));
        TutorialUpdateStepResponse response =
            new(new TutorialUpdateStepData(request.step, updateDataList));

        return Ok(response);
    }
}
