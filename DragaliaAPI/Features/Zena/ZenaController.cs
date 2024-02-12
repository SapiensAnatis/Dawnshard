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
    [HttpGet("get_team_data")]
    public async Task<ActionResult<GetTeamDataResponse>> GetTeamData(
        [FromQuery] long id,
        [FromQuery] int teamnum,
        [FromQuery] int teamnum2
    )
    {
        List<int> teamNumbers = [teamnum];
        if (teamnum2 != -1)
            teamNumbers.Add(teamnum2);

        using IDisposable impersonation = playerIdentityService.StartUserImpersonation(id);

        GetTeamDataResponse? response = await zenaService.GetTeamData(teamNumbers);

        if (response is null)
            return this.NotFound();

        return response;
    }
}
