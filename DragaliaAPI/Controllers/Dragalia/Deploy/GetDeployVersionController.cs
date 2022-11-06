using DragaliaAPI.Models.Responses;
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
        GetDeployVersionResponse response =
            new(new GetDeployVersionData(GetDeployVersionStatic.DeployHash));
        return Ok(response);
    }
}
