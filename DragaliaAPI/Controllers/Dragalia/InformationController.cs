using System.Net.Http.Headers;
using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("information")]
[Produces("application/json")]
[ApiController]
public class InformationController : ControllerBase
{
    [HttpGet]
    [Route("top")]
    public DragaliaResult InformationTop()
    {
        return Ok(new InformationTopResponse(InformationTopFactory.CreateData()));
    }
}
