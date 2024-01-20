using System.Security.Claims;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Website;

[Route("web/[controller]")]
public class LoginController(IPlayerIdentityService playerIdentityService) : ControllerBase
{
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;

    [HttpPost]
    [Authorize]
    public LoginResponse Login()
    {
        string playerName =
            this.User.FindFirstValue(CustomClaimType.PlayerName)
            ?? throw new InvalidOperationException("Failed to acquire player name");

        return new LoginResponse(this.playerIdentityService.ViewerId, playerName);
    }
}
