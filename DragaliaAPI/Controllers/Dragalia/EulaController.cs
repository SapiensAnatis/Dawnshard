using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Eula;

[Route("eula")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class EulaController : ControllerBase
{
    [HttpPost]
    [Route("get_version")]
    public DragaliaResult GetVersion(EulaGetVersionRequest request)
    {
        EulaVersion version =
            EulaStatic.AllEulaVersions.FirstOrDefault(
                x => x.region == request.region && x.lang == request.lang
            ) ?? EulaStatic.AllEulaVersions[0];

        EulaGetVersionResponse response = new(new EulaGetVersionData(version));
        return this.Ok(response);
    }

    [HttpPost]
    [Route("get_version_list")]
    public ActionResult<EulaGetVersionListResponse> GetVersionList()
    {
        EulaGetVersionListResponse response =
            new(new EulaGetVersionListData(EulaStatic.AllEulaVersions));
        return this.Ok(response);
    }
}
