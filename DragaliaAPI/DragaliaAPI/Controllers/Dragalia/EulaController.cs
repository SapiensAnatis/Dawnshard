using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("eula")]
[AllowAnonymous]
public class EulaController : DragaliaControllerBaseCore
{
    private static readonly List<AtgenVersionHash> AllEulaVersions =
        new()
        {
            // TODO: Add the complete list of versions
            new("gb", "en_us", 1, 1),
            new("gb", "en_eu", 1, 1),
            new("us", "en_us", 1, 6),
            new("us", "en_eu", 1, 6)
        };

    [HttpPost("get_version")]
    public DragaliaResult GetVersion(EulaGetVersionRequest request)
    {
        AtgenVersionHash version =
            AllEulaVersions.FirstOrDefault(x =>
                x.Region == request.Region && x.Lang == request.Lang
            ) ?? AllEulaVersions[0];

        return this.Ok(new EulaGetVersionResponse(version, false, 1));
    }

    [HttpPost("get_version_list")]
    public DragaliaResult GetVersionList()
    {
        return this.Ok(new EulaGetVersionListResponse(AllEulaVersions));
    }
}
