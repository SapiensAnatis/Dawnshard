using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Responses.Base;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

/// <summary>
/// This is presumably used to create a savefile on Dragalia's servers,
/// but we do that after creating a DeviceAccount in the Nintendo endpoint,
/// because we aren't limited by having two separate servers/DBs.
///
/// As a result, this controller just retrieves the existing savefile and
/// responds with its viewer_id.
/// </summary>
[Route("tool")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class ToolController : ControllerBase
{
    private readonly ISessionService sessionService;
    private readonly IUserDataRepository userDataRepository;

    public ToolController(ISessionService sessionService, IUserDataRepository userDataRepository)
    {
        this.sessionService = sessionService;
        this.userDataRepository = userDataRepository;
    }

    [HttpPost]
    [Route("signup")]
    public async Task<DragaliaResult> Signup(IdTokenRequest request)
    {
        long viewerId;

        try
        {
            string deviceAccountId = await this.sessionService.GetDeviceAccountId_IdToken(
                request.id_token
            );

            viewerId = await this.userDataRepository
                .GetUserData(deviceAccountId)
                .Select(x => x.ViewerId)
                .SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException or JsonException)
        {
            return this.Ok(new ServerErrorResponse());
        }

        SignupResponse response = new(new SignupData(viewerId));
        return this.Ok(response);
    }

    [HttpPost]
    [Route("get_service_status")]
    public ActionResult<ServiceStatusResponse> GetServiceStatus()
    {
        ServiceStatusResponse response = new(new ServiceStatusData(1));
        return this.Ok(response);
    }

    [HttpPost]
    [Route("auth")]
    public async Task<DragaliaResult> Auth(IdTokenRequest request)
    {
        string sessionId;
        long viewerId;

        try
        {
            sessionId = await this.sessionService.ActivateSession(request.id_token);
            string deviceAccountId = await this.sessionService.GetDeviceAccountId_SessionId(
                sessionId
            );
            IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
                deviceAccountId
            );
            viewerId = await playerInfo.Select(x => x.ViewerId).SingleAsync();
        }
        catch (Exception e) when (e is ArgumentException or JsonException)
        {
            return this.Ok(new ServerErrorResponse());
        }

        AuthResponseData data = new(viewerId, sessionId, "placeholder nonce");
        ToolAuthResponse response = new(data);
        return this.Ok(response);
    }
}
