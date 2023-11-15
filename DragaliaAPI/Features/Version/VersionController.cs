using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Version;

[Route("version")]
[AllowAnonymous]
[BypassResourceVersionCheck]
public class VersionController(IResourceVersionService resourceVersionService)
    : DragaliaControllerBase
{
    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetResourceVersion(VersionGetResourceVersionRequest request)
    {
        string resourceVersion = resourceVersionService.GetResourceVersion(request.platform);
        return this.Ok(new VersionGetResourceVersionData(resourceVersion));
    }
}
