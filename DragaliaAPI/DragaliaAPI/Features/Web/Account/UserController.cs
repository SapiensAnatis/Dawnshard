using System.Diagnostics;
using System.Security.Claims;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static DragaliaAPI.Features.Web.AuthConstants;

namespace DragaliaAPI.Features.Web.Account;

[Route("/api/user")]
[ApiController]
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
