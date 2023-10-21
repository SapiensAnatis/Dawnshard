using System.Security.Claims;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

[ApiController]
[SerializeException]
[ServiceFilter(typeof(ResourceVersionActionFilter))]
[Authorize(AuthenticationSchemes = SchemeName.Session)]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
public abstract class DragaliaControllerBase : ControllerBase
{
    protected string DeviceAccountId =>
        this.User.FindFirstValue(CustomClaimType.AccountId)
        ?? throw new InvalidOperationException("No AccountId claim value found");

    protected long ViewerId =>
        long.Parse(
            this.User.FindFirstValue(CustomClaimType.ViewerId)
                ?? throw new InvalidOperationException("No ViewerId claim value found")
        );

    public override OkObjectResult Ok(object? value)
    {
        return base.Ok(
            new DragaliaResponse<object>(
                value ?? throw new ArgumentNullException(nameof(value)),
                ResultCode.Success
            )
        );
    }

    public OkObjectResult Code(ResultCode code, string message)
    {
        return base.Ok(
            new DragaliaResponse<object>(
                data_headers: new DataHeaders(code),
                new ResultCodeData(code, message)
            )
        );
    }

    public OkObjectResult Code(ResultCode code)
    {
        return this.Code(code, string.Empty);
    }
}
