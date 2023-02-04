using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Shared;

namespace DragaliaAPI.Services;

[Obsolete(ObsoleteReasons.BaaS)]
public interface IDeviceAccountService
{
    Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount);

    Task<DeviceAccount> RegisterDeviceAccount();
}
