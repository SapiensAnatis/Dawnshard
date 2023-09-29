using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Version;

[Route("version")]
[AllowAnonymous]
public class VersionController(IOptionsMonitor<ResourceVersionOptions> options)
    : DragaliaControllerBase
{
    [HttpPost]
    [Route("get_resource_version")]
    public DragaliaResult GetResourceVersion(VersionGetResourceVersionRequest request)
    {
        string resourceVersion = request.platform switch
        {
            1 => options.CurrentValue.Ios,
            2 => options.CurrentValue.Android,
            _ => throw new BadHttpRequestException("Invalid platform identifier")
        };

        return this.Ok(new VersionGetResourceVersionData(resourceVersion));
    }
}
