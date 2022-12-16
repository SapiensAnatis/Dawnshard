using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Cygames;

[ApiController]
[NoSession]
[Route("api/v1")]
public class CygamesController : ControllerBase
{
    [HttpPost("Session")]
    public IActionResult Session()
    {
        return this.Ok();
    }

    [HttpPost("MeasurementEvent")]
    public IActionResult MeasurementEvent()
    {
        return this.Ok();
    }
}
