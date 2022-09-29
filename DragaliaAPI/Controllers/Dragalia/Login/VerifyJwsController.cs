using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Login;

[Route("login/verify_jws")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class VerifyJwsController : ControllerBase
{
    [HttpPost]
    public ActionResult<VerifyJwsResponse> Post()
    {
        return Ok(new OkResponse());
    }
}