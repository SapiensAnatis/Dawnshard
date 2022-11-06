using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Tool;

[Route("tool/get_service_status")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetServiceStatusController : ControllerBase
{
    [HttpPost]
    public ActionResult<ServiceStatusResponse> Post()
    {
        ServiceStatusResponse response = new(new ServiceStatusData(1));
        return Ok(response);
    }
}
