using DragaliaAPI.Models.Database.Savefile;
using Microsoft.EntityFrameworkCore;

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

        modelBuilder.Entity<DbSavefileUserData>()
            .Property(o => o.ViewerId)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.Viewer_id");
    }

    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

    public DbSet<DbSavefileUserData> SavefileUserData { get; set; } = null!;
}