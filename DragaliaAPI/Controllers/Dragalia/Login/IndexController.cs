using DragaliaAPI.Models.Base;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Login;

[Route("login/index")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class IndexController : ControllerBase
{
    [HttpPost]
    public ActionResult<OkResponse> Post()
    {
        return Ok(new OkResponse());
    }
}
