using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;

namespace DragaliaAPI.Controllers.Dragalia.Update;

[Route("update/namechange")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class NamechangeController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public NamechangeController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(
        [FromHeader(Name = "SID")] string sessionId,
        UpdateNamechangeRequest request
    )
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        await _apiRepository.UpdateName(deviceAccountId, request.name);

        UpdateNamechangeResponse response = new(new NamechangeData(request.name));
        return Ok(response);
    }
}
