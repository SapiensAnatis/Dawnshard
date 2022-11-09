using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers;

public abstract class DragaliaController : ControllerBase
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
