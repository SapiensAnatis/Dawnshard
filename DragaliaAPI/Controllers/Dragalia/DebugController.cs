using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("debug_controller")]
[Produces("application/octet-stream")]
[ApiController]
public class DebugController : DragaliaControllerBase
{
    [HttpGet]
    public object Get()
    {
        object result = new
        {
            String = "string",
            Int = 100.4,
            Bool = false,
            Bytes = new byte[] { 1, 2, 3, 4 },
            Object = new { Property = "value" },
            List = new List<object> { 1, 2, 3 }
        };
        return result;
    }
}
