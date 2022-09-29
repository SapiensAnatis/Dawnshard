using DragaliaAPI.Models;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Tool;

/// <summary>
/// This is presumably used to create a savefile on Dragalia's servers,
/// but we do that after creating a DeviceAccount in the Nintendo endpoint,
/// because we aren't limited by having two separate servers/DBs.
///
/// As a result, this controller just retrieves the existing savefile and
/// responds with its viewer_id.
/// </summary>
[Route("tool/signup")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class SignupController : ControllerBase
{
    private readonly ISessionService _sessionService;
    public SignupController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(IdTokenRequest request)
    {
        string? sessionId = _sessionService.GetSessionIdFromIdToken(request.id_token);
        if (sessionId is null) { return Ok(new ServerErrorResponse()); }

        DbPlayerSavefile savefile = await _sessionService.GetSavefile(sessionId);

        SignupResponse response = new(new SignupData(savefile.ViewerId));
        return Ok(response);
    }
}
