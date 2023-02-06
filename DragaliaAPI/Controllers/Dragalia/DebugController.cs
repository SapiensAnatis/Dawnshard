using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("debug_controller")]
[Produces("application/x-msgpack")]
[ApiController]
public class DebugController : ControllerBase
{
    [HttpGet]
    public object Get()
    {
        object result = new { List = new List<object> { 0, 0, 1, 0, 0 } };
        return this.Ok(result);
    }
}
