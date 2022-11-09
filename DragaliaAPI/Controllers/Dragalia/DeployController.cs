using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("deploy")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class DeployController : ControllerBase
{
    [HttpPost]
    [Route("get_deploy_version")]
    public DragaliaResult GetDeployVersion()
    {
        GetDeployVersionResponse response =
            new(new GetDeployVersionData(GetDeployVersionStatic.DeployHash));
        return this.Ok(response);
    }
}
