using System.Security.Claims;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

/// <summary>
/// Defines a controller for all Dawnshard API endpoints that implements the required metadata,
/// and which provides helpers to serialize Dragalia responses.
/// </summary>
/// <remarks>
/// For most controllers that are not involved in the title screen, <see cref="DragaliaControllerBase"/>
/// should be used.
/// </remarks>
[ApiController]
[SerializeException]
[Authorize(AuthenticationSchemes = SchemeName.Session)]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
public abstract class DragaliaControllerBaseCore : ControllerBase
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
                dataHeaders: new DataHeaders(code),
                new ResultCodeResponse(code, message)
            )
        );
    }

    public OkObjectResult Code(ResultCode code)
    {
        return this.Code(code, string.Empty);
    }
}

/// <summary>
/// Extends <see cref="DragaliaControllerBase"/> with extra action filters that can return the player to the title
/// screen under certain circumstances.
/// </summary>
/// <remarks>
/// Not to be used for endpoints that make up the title screen (/tool/*, /version/*, etc.) to prevent infinite loops.
/// </remarks>
[ServiceFilter<ResourceVersionActionFilter>]
[ServiceFilter<MaintenanceActionFilter>]
public abstract class DragaliaControllerBase : DragaliaControllerBaseCore { }
