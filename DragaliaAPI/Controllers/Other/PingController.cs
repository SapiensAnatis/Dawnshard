using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Other;

[Route("[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        return this.NoContent();
    }
}
