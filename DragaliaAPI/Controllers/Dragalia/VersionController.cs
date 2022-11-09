using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetResourceVersionController : ControllerBase
{
    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetVersionList()
    {
        GetResourceVersionResponse response =
            new(new GetResourceVersionData(GetResourceVersionStatic.ResourceVersion));
        return Ok(response);
    }
}
