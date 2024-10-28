using DragaliaAPI.Controllers;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Tool;

[AllowAnonymous]
[Route("tool")]
internal sealed class ToolController(IAuthService authService) : DragaliaControllerBaseCore
{
    [HttpPost]
    [Route("get_service_status")]
    public ActionResult<DragaliaResult> GetServiceStatus()
    {
        return this.Ok(new ToolGetServiceStatusResponse(1));
    }

    [HttpPost("signup", Name = "Signup")]
    [Authorize(AuthenticationSchemes = AuthConstants.SchemeNames.GameJwt)]
    public async Task<DragaliaResult> Signup()
    {
        long viewerId = await authService.DoSignup(this.User);

        return this.Ok(
            new ToolSignupResponse()
            {
                ViewerId = (ulong)viewerId,
                ServerTime = DateTimeOffset.UtcNow,
            }
        );
    }

    [HttpPost("auth")]
    [Authorize(AuthenticationSchemes = AuthConstants.SchemeNames.GameJwt)]
    public async Task<DragaliaResult> Auth()
    {
        if (!this.User.HasDawnshardIdentity())
        {
            // We can't rely on /tool/signup always being called for new users - as they may
            // have just switched from a different server with an initialized client
            await authService.DoSignup(this.User);
        }

        (long viewerId, string sessionId) = await authService.DoLogin(this.User);

        await authService.ImportSaveIfPending(this.User);

        return this.Ok(
            new ToolAuthResponse()
            {
                SessionId = sessionId,
                ViewerId = (ulong)viewerId,
                Nonce = "placeholder nonce",
            }
        );
    }

    [HttpPost("reauth")]
    [Authorize(AuthenticationSchemes = AuthConstants.SchemeNames.GameJwt)]
    public async Task<DragaliaResult> Reauth()
    {
        (long viewerId, string sessionId) = await authService.DoLogin(this.User);

        return this.Ok(
            new ToolReauthResponse()
            {
                SessionId = sessionId,
                ViewerId = (ulong)viewerId,
                Nonce = "placeholder nonce",
            }
        );
    }
}
