using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Savefile;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia.Load;

[Route("load/index")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class IndexController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public IndexController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post()
    {
        string sessionId = Request.Headers["SID"];
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbSavefileUserData dbUserData = await _apiRepository
            .GetPlayerInfo(deviceAccountId)
            .SingleAsync();
        SavefileUserData userData = SavefileUserDataFactory.Create(dbUserData, new() { });
        LoadIndexResponse response = new(new LoadIndexData(userData));

        return Ok(response);
    }
}
