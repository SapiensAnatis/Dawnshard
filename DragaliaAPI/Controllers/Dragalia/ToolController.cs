using System.IdentityModel.Tokens.Jwt;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
[AllowAnonymous]
public class ToolController : DragaliaControllerBase
{
    private readonly IAuthService authService;

    public ToolController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost]
    [Route("signup")]
    public async Task<DragaliaResult> Signup([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        (long viewerId, _) = await this.authService.DoAuth(idToken);

        return this.Ok(
            new ToolSignupData()
            {
                viewer_id = (ulong)viewerId,
                servertime = DateTimeOffset.UtcNow,
            }
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
    public async Task<DragaliaResult> Auth([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        // For some reason, the id_token in the ToolAuthRequest does not update with refreshes,
        // but the one in the header does.

        (long viewerId, string sessionId) = await this.authService.DoAuth(idToken);

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
    public async Task<DragaliaResult> Reauth([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        (long viewerId, string sessionId) = await this.authService.DoAuth(idToken);

        return this.Ok(
            new ToolReauthData()
            {
                session_id = sessionId,
                viewer_id = (ulong)viewerId,
                nonce = "placeholder nonce"
            }
        );
    }
}
