using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models
{
    public class DeviceAccountContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceAccountContext> _logger;
        public DeviceAccountContext (DbContextOptions<DeviceAccountContext> options, IConfiguration configuration, ILogger<DeviceAccountContext> logger) : base(options)
        {
            _configuration = configuration;
            _logger = logger;
            this.Database.CanConnect();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbDeviceAccount>().ToTable("DeviceAccount");
        }

        public DbSet<DbDeviceAccount> DeviceAccounts { get; set; }

        public async Task<bool> AuthenticateDeviceAccount(DeviceAccount deviceAccount)
        {
            if (deviceAccount.password is null) { throw new ArgumentNullException(paramName: deviceAccount.password); }

            DbDeviceAccount dbDeviceAccount = await DeviceAccounts.SingleAsync(x => x.Id == deviceAccount.id);
            string hashedPassword = GetHashedPassword(deviceAccount.password);

            return (hashedPassword == dbDeviceAccount.HashedPassword);
        }

        public async Task<DeviceAccount> RegisterDeviceAccount()
        {
            string id = Guid.NewGuid().ToString();
            string password = Guid.NewGuid().ToString();
            string hashedPassword = GetHashedPassword(password);

            DbDeviceAccount newDeviceAccount = new() { Id = id, HashedPassword = hashedPassword };
            await DeviceAccounts.AddAsync(newDeviceAccount);
            await this.SaveChangesAsync();

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
