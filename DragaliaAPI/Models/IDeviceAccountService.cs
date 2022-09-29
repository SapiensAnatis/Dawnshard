using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models;

public interface IDeviceAccountService
{
    Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount);
    Task<DeviceAccount> RegisterDeviceAccount();
}