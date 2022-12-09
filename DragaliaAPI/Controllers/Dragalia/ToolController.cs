using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
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
[NoSession]
[ApiController]
public class ToolController : DragaliaControllerBase
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
    public async Task<DragaliaResult> Signup(ToolSignupRequest request)
    {
        long viewerId;
        string deviceAccountId;

        try
        {
            deviceAccountId = await this.sessionService.GetDeviceAccountId_IdToken(
                request.id_token
            );
        }
        catch (SessionException)
        {
            return new OkObjectResult(
                new DragaliaResponse<ResultCodeData>(
                    new(ResultCode.COMMON_SESSION_RESTORE_ERROR),
                    ResultCode.COMMON_SESSION_RESTORE_ERROR
                )
            );
        }

        viewerId = await this.userDataRepository
            .GetUserData(deviceAccountId)
            .Select(x => x.ViewerId)
            .SingleAsync();

        return this.Ok(
            new ToolSignupData((ulong)viewerId, (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        );
    }

    [HttpPost]
    [Route("get_service_status")]
    public ActionResult<DragaliaResult> GetServiceStatus()
    {
        return this.Ok(new ToolGetServiceStatusData(1));
    }

    [HttpPost]
    [Route("auth")]
    public async Task<DragaliaResult> Auth(ToolAuthRequest request)
    {
        // return this.ResultCodeError(ResultCode.FORT_MAP_NOT_FREE);

        string sessionId;
        string deviceAccountId;
        long viewerId;

        try
        {
            sessionId = await this.sessionService.ActivateSession(request.id_token);
            deviceAccountId = await this.sessionService.GetDeviceAccountId_SessionId(sessionId);
        }
        catch (SessionException)
        {
            return new OkObjectResult(
                new DragaliaResponse<ResultCodeData>(
                    new(ResultCode.COMMON_SESSION_RESTORE_ERROR),
                    ResultCode.COMMON_SESSION_RESTORE_ERROR
                )
            );
        }

        IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
            deviceAccountId
        );
        viewerId = await playerInfo.Select(x => x.ViewerId).SingleAsync();

        ToolAuthData data = new((ulong)viewerId, sessionId, "placeholder nonce");
        return this.Ok(data);
    }
}
