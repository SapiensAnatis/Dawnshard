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
    public async Task<IActionResult> GetHeroParam(long viewerId)
    {
        return this.Ok(await this.heroParamService.GetHeroParam(viewerId));
    }
}
