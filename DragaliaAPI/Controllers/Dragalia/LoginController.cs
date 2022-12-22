using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("login")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class LoginController : DragaliaControllerBase
{
    [HttpPost]
    [Route("index")]
    public DragaliaResult Index()
    {
        // TODO: Implement daily login bonuses/notifications
        return Ok(new ResultCodeData(ResultCode.SUCCESS));
    }

    [HttpPost]
    [Route("verify_jws")]
    public DragaliaResult VerifyJws()
    {
        return Ok(new ResultCodeData(ResultCode.SUCCESS));
    }
}
