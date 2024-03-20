using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("tool")]
[AllowAnonymous]
public class ToolController(IAuthService authService) : DragaliaControllerBaseCore
{
    private const int OkServiceStatus = 1;
    private const int MaintenanceServiceStatus = 2;

    [HttpPost]
    [Route("signup")]
    public async Task<DragaliaResult> Signup([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        (long viewerId, _) = await authService.DoAuth(idToken);

        return this.Ok(
            new ToolSignupResponse()
            {
                ViewerId = (ulong)viewerId,
                ServerTime = DateTimeOffset.UtcNow,
            }
        );
    }

    [HttpPost]
    [Route("get_service_status")]
    public ActionResult<DragaliaResult> GetServiceStatus()
    {
        return this.Ok(new ToolGetServiceStatusResponse(1));
    }

    [HttpPost]
    [Route("auth")]
    public async Task<DragaliaResult> Auth([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        // For some reason, the id_token in the ToolAuthRequest does not update with refreshes,
        // but the one in the header does.

        (long viewerId, string sessionId) = await authService.DoAuth(idToken);

        return this.Ok(
            new ToolAuthResponse()
            {
                SessionId = sessionId,
                ViewerId = (ulong)viewerId,
                Nonce = "placeholder nonce"
            }
        );
    }

    [HttpPost("reauth")]
    public async Task<DragaliaResult> Reauth([FromHeader(Name = "ID-TOKEN")] string idToken)
    {
        (long viewerId, string sessionId) = await authService.DoAuth(idToken);

        return this.Ok(
            new ToolReauthResponse()
            {
                SessionId = sessionId,
                ViewerId = (ulong)viewerId,
                Nonce = "placeholder nonce"
            }
        );
    }
}
