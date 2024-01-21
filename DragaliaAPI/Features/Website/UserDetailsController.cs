using System.Security.Claims;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Website;

[Route("web/[controller]")]
public class UserDetailsController(IPlayerIdentityService playerIdentityService) : ControllerBase
{
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;

    [HttpGet]
    public UserDetailsResponse GetUserDetails()
    {
        string playerName =
            this.User.FindFirstValue(CustomClaimType.PlayerName)
            ?? throw new InvalidOperationException("Failed to acquire player name");

        return new UserDetailsResponse(this.playerIdentityService.ViewerId, playerName);
    }
}
