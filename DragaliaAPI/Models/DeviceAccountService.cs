using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Nintendo;
using Microsoft.Extensions.Caching.Distributed;



namespace DragaliaAPI.Models
{
    public class DeviceAccountService : IDeviceAccountService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DeviceAccount> _logger;
        private readonly IConfiguration _configuration;
        public DeviceAccountService(IDistributedCache cache, IConfiguration configuration, ILogger<DeviceAccount> logger)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
        {
            if (deviceAccount.password is null) { throw new ArgumentNullException(paramName: "deviceAccount.password"); }

            string hashedPassword = GetHashedPassword(deviceAccount.password);
            string storedPassword = await _cache.GetStringAsync($"DeviceAccount:{deviceAccount.id}:password");

            return (hashedPassword == storedPassword);
        }

        public async Task<DeviceAccount> RegisterDeviceAccount()
        {
            string id = Guid.NewGuid().ToString();
            string password = Guid.NewGuid().ToString();

            string hashedPassword = GetHashedPassword(password);

            await _cache.SetStringAsync($"DeviceAccount:{id}:password", hashedPassword);

            _logger.LogInformation("Registered new account with ID {id}", id);

            return new DeviceAccount(id, password);
            }

        private string GetHashedPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Dynamic salt would be better.
            // But security is not a top priority for this application; it is unlikely to be publically hosted for mass use.
            string salt = _configuration.GetValue<string>("HashSalt");
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            var pkbdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000);
            byte[] hashBytes = pkbdf2.GetBytes(20);
            
            return Convert.ToBase64String(hashBytes);
        }
    }
}
