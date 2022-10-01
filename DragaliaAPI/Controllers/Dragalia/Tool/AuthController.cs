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
    public AuthController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(IdTokenRequest request)
    {
        string sessionId;
        long viewerId;

        try
        {
            sessionId = await _sessionService.ActivateSession(request.id_token);
            IQueryable<DbSavefilePlayerInfo> savefile = await _sessionService.GetSavefile_SessionId(sessionId);
            viewerId = await savefile.Select(x => x.ViewerId).SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException || e is JsonException)
        {
            return Ok(new ServerErrorResponse());
        }

        AuthResponse response = new(viewerId, sessionId);
        return Ok(response);
    }
}