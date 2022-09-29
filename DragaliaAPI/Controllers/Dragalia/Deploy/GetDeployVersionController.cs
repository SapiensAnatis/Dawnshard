using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.Deploy;
[Route("deploy/get_deploy_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class GetDeployVersionController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post()
    {
        return Ok(new GetDeployVersionResponse());
    }
}
