using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DragaliaAPI.Features.Web.AuthConstants;

namespace DragaliaAPI.Features.Web.Users;

[ApiController]
[Route("/api/user")]
public class UserController(UserService userService, ILogger<UserController> logger)
    : ControllerBase
{
    [HttpGet("me")]
    [Authorize(Policy = PolicyNames.RequireValidJwt)]
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
}

file static class ClaimsPrincipalExtensions
{
    public static bool HasDawnshardIdentity(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Identities.Any(x => x.Label == IdentityLabels.Dawnshard);
}
