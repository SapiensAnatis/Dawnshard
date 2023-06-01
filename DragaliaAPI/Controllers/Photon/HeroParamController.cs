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

    [HttpGet("{viewerId}")]
    public async Task<IActionResult> GetHeroParam(
        long viewerId,
        [FromQuery] int partySlot1,
        [FromQuery] int? partySlot2
    )
    {
        List<int> partySlots = new() { partySlot1 };
        if (partySlot2 is not null)
            partySlots.Add(partySlot2.Value);

        return this.Ok(await this.heroParamService.GetHeroParam(viewerId, partySlots));
    }
}
