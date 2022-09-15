using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models
{
    public class DeviceAccountService : IDeviceAccountService
    {
        private readonly DeviceAccountContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceAccountService> _logger;

        public DeviceAccountService(DeviceAccountContext context, IConfiguration configuration, ILogger<DeviceAccountService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
        {
            if (deviceAccount.password is null) { throw new ArgumentNullException(paramName: deviceAccount.password); }

            DbDeviceAccount? dbDeviceAccount = await _context.DeviceAccounts.SingleOrDefaultAsync(x => x.Id == deviceAccount.id);
            if (dbDeviceAccount is null) { return false; }

            string hashedPassword = GetHashedPassword(deviceAccount.password);

            return (hashedPassword == dbDeviceAccount.HashedPassword);
        }

        public async Task<DeviceAccount> RegisterDeviceAccount()
        {
            string id = Guid.NewGuid().ToString();
            string password = Guid.NewGuid().ToString();
            string hashedPassword = GetHashedPassword(password);

            DbDeviceAccount newDeviceAccount = new(id, hashedPassword);
            await _context.DeviceAccounts.AddAsync(newDeviceAccount);
            await _context.SaveChangesAsync();

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