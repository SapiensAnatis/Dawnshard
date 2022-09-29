using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Eula;

[Route("eula/get_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class EulaGetVersionController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post(EulaGetVersionRequest request)
    {
        EulaVersion version =
            EulaData.AllEulaVersions.FirstOrDefault(x => x.region == request.region && x.lang == request.lang) ??
            EulaData.AllEulaVersions[0];

        EulaGetVersionResponse response = new(version);
        return Ok(response);
    }
}
