using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
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
    private readonly IApiRepository _apiRepository;

    public AuthController(ISessionService sessionService, IApiRepository repository)
    {
        _sessionService = sessionService;
        _apiRepository = repository;
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
            IQueryable<DbSavefileUserData> playerInfo = _apiRepository.GetPlayerInfo(deviceAccountId);
            viewerId = await playerInfo.Select(x => x.ViewerId).SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException || e is JsonException)
        {
            return Ok(new ServerErrorResponse());
        }

        AuthResponseData data = new(viewerId, sessionId, "placeholder nonce");
        AuthResponse response = new(data);
        return Ok(response);
    }
}