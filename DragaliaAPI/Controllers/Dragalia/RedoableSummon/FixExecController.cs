using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.RedoableSummon;

[Route("redoable_summon/fix_exec")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FixExecController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post()
    {
        // TODO: Fill response data from cached results and new/converted unit lists
        RedoableSummonFixExecResponse response = new(RedoableSummonFixExecFactory.CreateData());
        return Ok(response);
    }
}
