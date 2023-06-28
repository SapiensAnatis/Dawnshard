using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Services.Photon;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Photon;

[Route("[controller]")]
public class HeroParamController : ControllerBase
{
    private readonly IHeroParamService heroParamService;

    public HeroParamController(IHeroParamService heroParamService)
    {
        this.heroParamService = heroParamService;
    }

    [HttpGet("{viewerId}/{partySlot}")]
    public async Task<IActionResult> GetHeroParam(long viewerId, int partySlot)
    {
        return this.Ok(await this.heroParamService.GetHeroParam(viewerId, partySlot));
    }

    [HttpPost("batch")]
    public async Task<IActionResult> GetHeroParam([FromBody] IEnumerable<ActorInfo> request)
    {
        List<HeroParamData> response = new();

        foreach (ActorInfo actorInfo in request)
        {
            HeroParamData data =
                new() { ActorNr = actorInfo.ActorNr, ViewerId = actorInfo.ViewerId };

            foreach (int partySlot in actorInfo.PartySlots)
            {
                data.HeroParamLists.Add(
                    await this.heroParamService.GetHeroParam(actorInfo.ViewerId, partySlot)
                );
            }

            response.Add(data);
        }

        return this.Ok(response);
    }
}
