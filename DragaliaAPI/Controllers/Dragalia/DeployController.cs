using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("deploy")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class DeployController : DragaliaControllerBase
{
    private const string DeployHash = "13bb2827ce9e6a66015ac2808112e3442740e862";

    [HttpPost]
    [Route("get_deploy_version")]
    public DragaliaResult GetDeployVersion()
    {
        return this.Ok(new DeployGetDeployVersionData(DeployHash));
    }
}
