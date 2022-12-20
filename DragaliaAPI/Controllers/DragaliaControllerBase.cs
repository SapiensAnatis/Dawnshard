using DragaliaAPI.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

[ApiController]
[Consumes("application/octet-stream")]
[Produces("application/x-msgpack")]
public abstract class DragaliaControllerBase : ControllerBase
{
    private string? deviceAccountId = null;

    protected string DeviceAccountId
    {
        get
        {
            this.deviceAccountId ??= this.LoadDeviceAccountId();
            return this.deviceAccountId;
        }
    }

    public override OkObjectResult Ok(object? value)
    {
        return base.Ok(
            new DragaliaResponse<object>(
                value ?? throw new ArgumentNullException(nameof(value)),
                ResultCode.SUCCESS
            )
        );
    }

    public OkObjectResult ResultCodeError(ResultCode value)
    {
        return base.Ok(new DragaliaResponse<ResultCodeData>(new(value), value));
    }

    private string LoadDeviceAccountId()
    {
        if (
            this.HttpContext.Items.TryGetValue("DeviceAccountId", out object? deviceAccountId)
            && deviceAccountId is not null
        )
        {
            return (string)deviceAccountId;
        }

        throw new SessionException("Internal controller session lookup error");
    }
}
