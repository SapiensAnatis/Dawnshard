using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Controllers.Dragalia.Eula;

[Route("eula/get_version_list")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetVersionListController : ControllerBase
{
    [HttpPost]
    public ActionResult<EulaGetVersionListResponse> Post()
    {
        EulaGetVersionListResponse response =
            new(new EulaGetVersionListData(EulaStatic.AllEulaVersions));
        return Ok(response);
    }
}
