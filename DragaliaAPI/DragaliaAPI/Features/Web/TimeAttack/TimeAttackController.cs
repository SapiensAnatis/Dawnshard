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
    public async Task<List<object>> GetRankings(int questId) =>
        await timeAttackService.GetRankings(questId);
}
