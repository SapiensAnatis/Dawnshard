using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        ServiceStatusResponse response = new();
        return Ok(response);
    }
}