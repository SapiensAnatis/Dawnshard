using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Version;

[Route("version")]
[AllowAnonymous]
public class VersionController(IResourceVersionService resourceVersionService)
    : DragaliaControllerBaseCore
{
    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetResourceVersion(VersionGetResourceVersionRequest request)
    {
        string resourceVersion = resourceVersionService.GetResourceVersion(request.Platform);
        return this.Ok(new VersionGetResourceVersionResponse(resourceVersion));
    }
}
