using DragaliaAPI.Services.Exceptions;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Controllers.Other;

/// <summary>
/// Used in the middleware tests: a controller whose expected response should never change
/// </summary>
#if DEBUG
[Route("test")]
[Produces("text/plain")]
public class TestController : DragaliaControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return new OkObjectResult("OK");
    }

    [HttpGet("anonymous")]
    [AllowAnonymous]
    public IActionResult Anonymous()
    {
        return new OkObjectResult("OK");
    }

    [HttpGet("taskcancelled")]
    public IActionResult TaskCancelled()
    {
        throw new TaskCanceledException();
    }

    [HttpGet("messagepackserialization/taskcancelled")]
    public IActionResult MessagePackSerialization_TaskCancelled()
    {
        throw new MessagePackSerializationException("test", new TaskCanceledException());
    }

    [HttpGet("messagepackserialization/operationcancelled")]
    public IActionResult MessagePackSerialization_OperationCancelled()
    {
        throw new MessagePackSerializationException("test", new OperationCanceledException());
    }

    [HttpPost("dragalia")]
    public IActionResult Dragalia()
    {
        throw new DragaliaException(ResultCode.AbilityCrestBuildupPieceShortLevel);
    }

    [HttpPost("securitytokenexpired")]
    public IActionResult SecurityTokenExpired()
    {
        throw new SecurityTokenExpiredException();
    }
}
#endif
