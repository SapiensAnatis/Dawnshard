using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
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
[NoSession]
[Route("tool")]
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
        string deviceAccountId = await this.sessionService.GetDeviceAccountId_IdToken(
            request.id_token
        );

        long viewerId = await this.userDataRepository
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
        (long viewerId, string sessionId) = await this.DoAuth(request.id_token);

        return this.Ok(
            new ToolAuthData()
            {
                session_id = sessionId,
                viewer_id = (ulong)viewerId,
                nonce = "placeholder nonce"
            }
        );
    }

    [HttpPost("reauth")]
    public async Task<DragaliaResult> Reauth(ToolReauthRequest request)
    {
        (long viewerId, string sessionId) = await this.DoAuth(request.id_token);

        return this.Ok(
            new ToolReauthData()
            {
                session_id = sessionId,
                viewer_id = (ulong)viewerId,
                nonce = "placeholder nonce"
            }
        );
    }

    private async Task<(long viewerId, string sessionId)> DoAuth(string idToken)
    {
        string sessionId;
        string deviceAccountId;

        sessionId = await this.sessionService.ActivateSession(idToken);
        deviceAccountId = await this.sessionService.GetDeviceAccountId_SessionId(sessionId);

        IQueryable<DbPlayerUserData> playerInfo = this.userDataRepository.GetUserData(
            deviceAccountId
        );

        return (await playerInfo.Select(x => x.ViewerId).SingleAsync(), sessionId);
    }
}
