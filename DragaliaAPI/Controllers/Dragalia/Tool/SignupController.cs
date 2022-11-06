using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Services;
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
    private readonly IUserDataRepository userDataRepository;

    public SignupController(ISessionService sessionService, IUserDataRepository userDataRepository)
    {
        _sessionService = sessionService;
        this.userDataRepository = userDataRepository;
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
            IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
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
