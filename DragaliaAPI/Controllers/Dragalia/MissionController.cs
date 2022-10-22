using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mission")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MissionController : ControllerBase
{
    [HttpPost]
    [Route("get_mission_list")]
    public DragaliaResult GetMissionList()
    {
        return Ok(new GetMissionListResponse(GetMissionListFactory.CreateData()));
    }
}
