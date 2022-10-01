using System.Security.Cryptography;
using System.Text;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Models.Database;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    public ApiContext() : base()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<long>("Viewer_id", schema: "dbo")
            .StartsAt(10000000000L)
            .IncrementsBy(1);

        modelBuilder.Entity<DbSavefilePlayerInfo>()
            .Property(o => o.ViewerId)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.Viewer_id");
    }

    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

    public DbSet<DbSavefilePlayerInfo> PlayerSavefiles { get; set; } = null!;
}