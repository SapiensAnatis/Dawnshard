using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Controllers.Nintendo;

[Route("bigdata/v1/analytics/events/config")]
[Consumes("application/json")]
[Produces("application/json")]
[ApiController]
public class AnalyticsConfigController : ControllerBase
{
    [HttpGet]
    public ActionResult<AnalyticsConfigResponse> Get()
    {
        return Ok(new AnalyticsConfigResponse());
    }
}

[Route("bigdata/v1/analytics")]
[Consumes("application/json")]
[Produces("text/html")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    [HttpPost]
    public ActionResult Post()
    {
        return StatusCode(202);
    }
}
