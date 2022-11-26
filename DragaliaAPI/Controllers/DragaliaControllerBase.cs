using DragaliaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

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
                ResultCode.Success
            )
        );
    }

    private string LoadDeviceAccountId()
    {
        if (
            !this.HttpContext.Items.TryGetValue("DeviceAccountId", out object? deviceAccountId)
            || deviceAccountId is null
        )
        {
            throw new BadHttpRequestException("Session lookup error");
        }

        return (string)deviceAccountId;
    }
}
