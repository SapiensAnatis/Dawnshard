using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Controllers.Other;

[Route("dragalipatch")]
[ApiController]
public class DragalipatchController : ControllerBase
{
    private readonly IOptionsMonitor<DragalipatchOptions> patchOptions;
    private readonly IOptionsMonitor<LoginOptions> loginOptions;

    public DragalipatchController(
        IOptionsMonitor<DragalipatchOptions> patchOptions,
        IOptionsMonitor<LoginOptions> loginOptions
    )
    {
        this.patchOptions = patchOptions;
        this.loginOptions = loginOptions;
    }

    [HttpGet("config")]
    public ActionResult<DragalipatchResponse> Config()
    {
        return this.Ok(
            new DragalipatchResponse(this.loginOptions.CurrentValue, this.patchOptions.CurrentValue)
        );
    }
}
