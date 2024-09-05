using DragaliaAPI.Features.Web.TimeAttack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Web.TimeAttack;

[ApiController]
[Route("/api/time_attack")]
[AllowAnonymous]
internal sealed class TimeAttackController(TimeAttackService timeAttackService) : ControllerBase
{
    [HttpGet("quests")]
    public async Task<ActionResult<List<TimeAttackQuest>>> GetQuests() =>
        await timeAttackService.GetQuests();

    [HttpGet("rankings/{questId:int}")]
    public async Task<OffsetPagedResponse<TimeAttackRanking>> GetRankings(
        int questId,
        [FromQuery] int offset = 0,
        [FromQuery] int pageSize = 10
    ) => await timeAttackService.GetRankings(questId, offset, pageSize);
}
