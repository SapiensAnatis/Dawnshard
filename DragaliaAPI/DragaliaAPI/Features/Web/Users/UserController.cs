using DragaliaAPI.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DragaliaAPI.Infrastructure.Authentication.AuthConstants;

namespace DragaliaAPI.Features.Web.Users;

[ApiController]
[Route("/api/user")]
internal sealed class UserController(UserService userService, ILogger<UserController> logger)
    : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Policy = PolicyNames.RequireValidWebJwt)]
    public async Task<ActionResult<User>> GetSelf(CancellationToken cancellationToken)
    {
        if (!this.User.HasDawnshardIdentity())
        {
            logger.LogInformation("User does not have a Dawnshard identity.");
            return this.NotFound();
        }

        return await userService.GetUser(cancellationToken);
    }

    [HttpGet("me/profile")]
    [Authorize(Policy = PolicyNames.RequireDawnshardIdentity)]
    public async Task<ActionResult<UserProfile>> GetSelfProfile(
        CancellationToken cancellationToken
    ) => await userService.GetUserProfile(cancellationToken);

    [HttpGet("me/impersonation_session")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<ActionResult<ImpersonationSession>> GetImpersonationSession(
        CancellationToken cancellationToken
    ) => await userService.GetImpersonationSession(cancellationToken);

    [HttpPut("me/impersonation_session")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task<ActionResult<ImpersonationSession>> SetImpersonationSession(
        [FromForm] long impersonatedViewerId,
        CancellationToken cancellationToken
    )
    {
        string? impersonatedAccountId = await userService.GetImpersonationTargetAccountId(
            impersonatedViewerId,
            cancellationToken
        );

        if (impersonatedAccountId is null)
        {
            // Target player does not exist
            return NotFound();
        }

        return await userService.SetImpersonationSession(
            impersonatedAccountId,
            impersonatedViewerId,
            cancellationToken
        );
    }

    [HttpDelete("me/impersonation_session")]
    [Authorize(Policy = PolicyNames.RequireAdmin)]
    public async Task ClearImpersonationSession(CancellationToken cancellationToken) =>
        await userService.ClearImpersonationSession(cancellationToken);
}
