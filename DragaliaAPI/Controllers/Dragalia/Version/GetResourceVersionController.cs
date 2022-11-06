using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Version;

[Route("version/get_resource_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetResourceVersionController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post()
    {
        GetResourceVersionResponse response =
            new(new GetResourceVersionData(GetResourceVersionStatic.ResourceVersion));
        return Ok(response);
    }
}
