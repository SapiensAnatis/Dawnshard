using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DragaliaAPI.Controllers.Dragalia.Tool;

[Route("tool/auth")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IUserDataRepository userDataRepository;

    public AuthController(ISessionService sessionService, IUserDataRepository userDataRepository)
    {
        _sessionService = sessionService;
        this.userDataRepository = userDataRepository;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(IdTokenRequest request)
    {
        string sessionId;
        long viewerId;

        try
        {
            sessionId = await _sessionService.ActivateSession(request.id_token);
            string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
                deviceAccountId
            );
            viewerId = await playerInfo.Select(x => x.ViewerId).SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException or JsonException)
        {
            return Ok(new ServerErrorResponse());
        }

        AuthResponseData data = new(viewerId, sessionId, "placeholder nonce");
        ToolAuthResponse response = new(data);
        return Ok(response);
    }
}
