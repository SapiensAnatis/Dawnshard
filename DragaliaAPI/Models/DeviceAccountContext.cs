using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models
{
    public class DbDeviceAccount
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string HashedPassword { get; set; }

        public DbDeviceAccount(string id, string hashedPassword)
        {
            Id = id;
            HashedPassword = hashedPassword;
        }
    }

    public class DeviceAccountContext : DbContext
    {
        public DeviceAccountContext(DbContextOptions<DeviceAccountContext> options) : base(options)
        {
        }

        public DeviceAccountContext()
        {
        }

        public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

        public virtual async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
        {
            return await this.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task AddNewDeviceAccount(string id, string hashedPassword)
        {
            await this.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
            await this.SaveChangesAsync();
        }
    }
}
