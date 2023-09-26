using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("version")]
[AllowAnonymous]
public class VersionController : DragaliaControllerBase
{
    private const string AndroidResourceVersion = "y2XM6giU6zz56wCm";
    private const string IosResourceVersion = "h36dI3Buk9ltomn5";

    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetResourceVersion(VersionGetResourceVersionRequest request)
    {
        string resourceVersion = request.platform switch
        {
            1 => IosResourceVersion,
            2 => AndroidResourceVersion,
            _ => throw new BadHttpRequestException("Invalid platform identifier")
        };

        return this.Ok(new VersionGetResourceVersionData(resourceVersion));
    }
}
