using DragaliaAPI.Models.Responses.Base;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("login")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class LoginController : ControllerBase
{
    [HttpPost]
    [Route("index")]
    public ActionResult<OkResponse> Index()
    {
        // TODO: Implement daily login bonuses/notifications
        return Ok(new OkResponse());
    }

    [HttpPost]
    [Route("verify_jws")]
    public ActionResult<OkResponse> VerifyJws()
    {
        return Ok(new OkResponse());
    }
}
