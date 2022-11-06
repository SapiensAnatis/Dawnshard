﻿using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

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
