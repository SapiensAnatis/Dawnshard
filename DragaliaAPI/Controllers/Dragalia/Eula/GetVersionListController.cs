using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Responses;

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
        return Ok(new EulaGetVersionListResponse());
    }
}