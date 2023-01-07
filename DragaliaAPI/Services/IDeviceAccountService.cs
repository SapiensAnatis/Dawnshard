using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Services;

[Obsolete("Used for pre-BaaS login flow")]
public interface IDeviceAccountService
{
    Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount);

    Task<DeviceAccount> RegisterDeviceAccount();
}
