using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Authentication;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DragaliaAPI.Infrastructure.Authentication.AuthConstants;

namespace DragaliaAPI.Features.Login.Auth;

[Route("tool")]
internal sealed class ToolController(IAuthService authService) : DragaliaControllerBaseCore
{
    [HttpPost]
    [AllowAnonymous]
    [Route("get_service_status")]
    public ActionResult<DragaliaResult> GetServiceStatus()
    {
        return this.Ok(new ToolGetServiceStatusResponse(1));
    }

    [HttpPost("signup")]
    [Authorize(AuthenticationSchemes = SchemeNames.GameJwt)]
    public async Task<DragaliaResult> Signup()
    {
        if (this.User.HasDawnshardIdentity())
        {
            throw new DragaliaException(
                ResultCode.CommonAuthError,
                "Unable to perform signup: user already has an account"
            );
        }

        DbPlayer player = await authService.DoSignup(this.User);

        return this.Ok(
            new ToolSignupResponse()
            {
                ViewerId = (ulong)player.ViewerId,
                ServerTime = DateTimeOffset.UtcNow,
            }
        );
    }

    [HttpPost("auth")]
    [Authorize(AuthenticationSchemes = SchemeNames.GameJwt)]
    public async Task<DragaliaResult> Auth()
    {
        if (!this.User.HasDawnshardIdentity())
        {
            // We can't rely on /tool/signup always being called for new users - as they may
            // have just switched from a different server with an initialized client
            DbPlayer player = await authService.DoSignup(this.User);
            this.User.InitializeDawnshardIdentity(player.AccountId, player.ViewerId);
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
    [Authorize(AuthenticationSchemes = SchemeNames.GameJwt)]
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
