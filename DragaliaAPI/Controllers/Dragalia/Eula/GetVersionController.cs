using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Eula;

[Route("eula/get_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetVersionController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post(EulaGetVersionRequest request)
    {
        EulaVersion version =
            EulaStatic.AllEulaVersions.FirstOrDefault(x => x.region == request.region && x.lang == request.lang) ??
            EulaStatic.AllEulaVersions[0];

        EulaGetVersionResponse response = new(new EulaGetVersionData(version));
        return Ok(response);
    }
}
