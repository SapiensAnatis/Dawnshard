using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Database.Repositories;

namespace DragaliaAPI.Controllers.Dragalia.Update;

[Route("update/namechange")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class NamechangeController : ControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly ISessionService _sessionService;

    public NamechangeController(
        IUserDataRepository userDataRepository,
        ISessionService sessionService
    )
    {
        this.userDataRepository = userDataRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(
        [FromHeader(Name = "SID")] string sessionId,
        UpdateNamechangeRequest request
    )
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        await userDataRepository.UpdateName(deviceAccountId, request.name);

        UpdateNamechangeResponse response = new(new NamechangeData(request.name));
        return Ok(response);
    }
}
