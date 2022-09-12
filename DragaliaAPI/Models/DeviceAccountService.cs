using StackExchange.Redis;
using DragaliaAPI.Models.Nintendo;

namespace DragaliaAPI.Models
{
    public class DeviceAccountService : IDeviceAccountService
    {
        private readonly IConnectionMultiplexer _redis;

        public DeviceAccountService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceAccount> RegisterDeviceAccount()
        {
            throw new NotImplementedException();
        }
    }
}
