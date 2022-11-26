using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("friend")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FriendController : ControllerBase
{
    [HttpPost]
    [Route("get_support_chara")]
    public DragaliaResult GetSupportChara()
    {
        return Ok(
            new FriendGetSupportCharaData(
                0,
                new SettingSupport(Charas.ThePrince, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            )
        );
    }
}
