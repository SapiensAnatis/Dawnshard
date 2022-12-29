using DragaliaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Controllers.Other;

[Route("dragalipatch")]
[ApiController]
public class DragalipatchController : ControllerBase
{
    private readonly IOptionsMonitor<DragalipatchConfig> options;

    public DragalipatchController(IOptionsMonitor<DragalipatchConfig> options)
    {
        this.options = options;
    }

    [HttpGet("config")]
    public IActionResult Config()
    {
        return this.Ok(this.options.CurrentValue);
    }
}
