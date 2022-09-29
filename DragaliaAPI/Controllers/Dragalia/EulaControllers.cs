using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Responses;

namespace DragaliaAPI.Controllers.Dragalia
{
    [Route("eula/get_version_list")]
    [Consumes("application/octet-stream")]
    [Produces("application/octet-stream")]
    [ApiController]
    public class EulaGetVersionListController : ControllerBase
    {
        [HttpPost]
        public ActionResult<EulaGetVersionListResponse> Post()
        {
            return Ok(new EulaGetVersionListResponse());
        }
    }

    [Route("eula/get_version")]
    [Consumes("application/octet-stream")]
    [Produces("application/octet-stream")]
    [ApiController]
    public class EulaGetVersionController : ControllerBase
    {
        [HttpPost]
        public ActionResult<BaseResponse<object>> Post(EulaGetVersionRequest request)
        {
            EulaVersion version = 
                EulaData.AllEulaVersions.FirstOrDefault(x => x.region == request.region && x.lang == request.lang) ?? 
                EulaData.AllEulaVersions[0];

            EulaGetVersionResponse response = new(version);
            return Ok(response);
        }
    }
}
