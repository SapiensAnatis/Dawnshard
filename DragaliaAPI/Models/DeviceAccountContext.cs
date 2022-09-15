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
        private readonly IConfiguration _configuration;
        public DeviceAccountContext(DbContextOptions<DeviceAccountContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbDeviceAccount>().ToTable("DeviceAccount");
        }

        public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;
    }
}
