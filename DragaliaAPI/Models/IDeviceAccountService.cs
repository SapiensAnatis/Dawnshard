using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Models
{
    public interface IDeviceAccountService
    {
        Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount);
        Task<DeviceAccount> RegisterDeviceAccount();
    }
}