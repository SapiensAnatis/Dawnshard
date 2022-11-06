using DragaliaAPI.Models.Base;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Login;

[Route("login/verify_jws")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class VerifyJwsController : ControllerBase
{
    [HttpPost]
    public ActionResult<OkResponse> Post()
    {
        return Ok(new OkResponse());
    }
}
