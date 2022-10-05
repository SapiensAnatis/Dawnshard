using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.RedoableSummon;

[Route("redoable_summon/pre_exec")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PreExecController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post()
    {
        // TODO: Make it actually execute a randomized summon
        RedoableSummonPreExecResponse response = new(RedoableSummonPreExecFactory.CreateData());
        return Ok(response);
    }
}
