using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models.Database
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        public ApiContext()
        {
        }

        public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

        public DbSet<DbPlayerSavefile> PlayerSavefiles { get; set; } = null!;
    }
}
