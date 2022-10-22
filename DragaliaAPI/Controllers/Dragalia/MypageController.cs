using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Responses.Common;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MypageController : ControllerBase
{
    [Route("info")]
    [HttpPost]
    public DragaliaResult Info()
    {
        return Ok(new OkResponse());
    }
}
