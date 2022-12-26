using System.Security.Claims;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

[ApiController]
[SerializeException]
[Authorize(AuthenticationSchemes = SchemeName.Session)]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
public abstract class DragaliaControllerBase : ControllerBase
{
    protected string DeviceAccountId =>
        this.User.FindFirstValue(CustomClaimType.AccountId)
        ?? throw new InvalidOperationException("No AccountId claim value found");

    public override OkObjectResult Ok(object? value)
    {
        return base.Ok(
            new DragaliaResponse<object>(
                value ?? throw new ArgumentNullException(nameof(value)),
                ResultCode.SUCCESS
            )
        );
    }
}
