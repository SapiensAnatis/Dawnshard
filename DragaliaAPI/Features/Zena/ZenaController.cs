using DragaliaAPI.Middleware;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Zena;

[Route("[controller]")]
[Authorize(AuthenticationSchemes = SchemeName.Zena)]
public class ZenaController(IPlayerIdentityService playerIdentityService, IZenaService zenaService)
    : ControllerBase
{
    private readonly IPlayerIdentityService playerIdentityService = playerIdentityService;
    private readonly IZenaService zenaService = zenaService;

    [HttpGet("get_team_data")]
    public async Task<GetTeamDataResponse> GetTeamData(
        [FromQuery] long id,
        [FromQuery] int teamnum,
        [FromQuery] int teamnum2
    )
    {
        List<int> teamNumbers = [teamnum];
        if (teamnum2 != -1)
            teamNumbers.Add(teamnum2);

        using IDisposable impersonation = this.playerIdentityService.StartUserImpersonation(id);

        return await this.zenaService.GetTeamData(teamNumbers);
    }
}
