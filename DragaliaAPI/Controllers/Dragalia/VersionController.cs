using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetResourceVersionController : DragaliaControllerBase
{
    private const string ResourceVersion = "y2XM6giU6zz56wCm";

    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetVersionList()
    {
        return this.Ok(new VersionGetResourceVersionData(ResourceVersion));
    }
}
