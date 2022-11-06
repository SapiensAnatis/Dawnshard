using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Services;

public interface IDeviceAccountService
{
    Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount);

    Task<DeviceAccount> RegisterDeviceAccount();
}
