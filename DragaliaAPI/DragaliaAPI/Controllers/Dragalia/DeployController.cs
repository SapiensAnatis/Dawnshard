using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("deploy")]
[AllowAnonymous]
public class DeployController : DragaliaControllerBaseCore
{
    private const string DeployHash = "13bb2827ce9e6a66015ac2808112e3442740e862";

    [HttpPost]
    [Route("get_deploy_version")]
    public DragaliaResult GetDeployVersion()
    {
        return this.Ok(new DeployGetDeployVersionResponse(DeployHash));
    }
}
