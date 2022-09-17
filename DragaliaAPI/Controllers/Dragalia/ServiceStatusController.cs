using DragaliaAPI.Models.Dragalia;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DragaliaAPI.Controllers.Dragalia
{
    [Route("tool/get_service_status")]
    [Consumes("application/octet-stream")]
    [Produces("application/octet-stream")]
    [ApiController]
    public class ServiceStatusController : ControllerBase
    {
        [HttpPost]
        public ActionResult<ServiceStatusResponse> Post()
        {
            ServiceStatusResponse response = new();
            return Ok(response);
        }
    }
}
