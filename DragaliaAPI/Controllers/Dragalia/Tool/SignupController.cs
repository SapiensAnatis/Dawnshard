using System.Text.Json;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly IApiRepository _apiRepository;

    public SignupController(ISessionService sessionService, IApiRepository apiRepository)
    {
        _sessionService = sessionService;
        _apiRepository = apiRepository;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(IdTokenRequest request)
    {
        long viewerId;

        try
        {
            string deviceAccountId = await _sessionService.GetDeviceAccountId_IdToken(
                request.id_token
            );
            IQueryable<DbSavefileUserData> playerInfo = _apiRepository.GetPlayerInfo(
                deviceAccountId
            );
            viewerId = await playerInfo.Select(x => x.ViewerId).SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException || e is JsonException)
        {
            return Ok(new ServerErrorResponse());
        }

        SignupResponse response = new(new SignupData(viewerId));
        return Ok(response);
    }
}
