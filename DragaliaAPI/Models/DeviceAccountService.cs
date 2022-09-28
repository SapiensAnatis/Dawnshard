using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models
{
    public class DeviceAccountService : IDeviceAccountService
    {
        private readonly IApiRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceAccountService> _logger;

        public DeviceAccountService(IApiRepository repository, IConfiguration configuration, ILogger<DeviceAccountService> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
        {
            if (deviceAccount.password is null) { throw new ArgumentNullException(paramName: deviceAccount.password); }

            DbDeviceAccount? dbDeviceAccount = await _repository.GetDeviceAccountById(deviceAccount.id);
            if (dbDeviceAccount is null) { return false; }

            string hashedPassword = GetHashedPassword(deviceAccount.password);

            return (hashedPassword == dbDeviceAccount.HashedPassword);
        }

        public async Task<DeviceAccount> RegisterDeviceAccount()
        {
            string id = Guid.NewGuid().ToString();
            string password = Guid.NewGuid().ToString();
            string hashedPassword = GetHashedPassword(password);

            await _repository.AddNewDeviceAccount(id, hashedPassword);
            await _repository.AddNewPlayerSavefile(id);

            _logger.LogInformation("Registered new account with ID {id}", id);

            return new DeviceAccount(id, password);
        }

        private string GetHashedPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Dynamic salt would be better.
            // But security is not a top priority for this application; it is unlikely to be publically hosted for mass use.
            byte[] saltBytes = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("HashSalt"));

            var pkbdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000);
            byte[] hashBytes = pkbdf2.GetBytes(20);

            return Convert.ToBase64String(hashBytes);
        }
    }
} 