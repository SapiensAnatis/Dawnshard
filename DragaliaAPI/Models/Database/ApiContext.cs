using DragaliaAPI.Models.Database.Savefile;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models.Database;

public class ApiContext : DbContext
{
    private readonly bool _isDevelopment;

    public ApiContext(DbContextOptions<ApiContext> options, IWebHostEnvironment environment)
        : base(options)
    {
        _isDevelopment = environment.IsDevelopment();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // The SQLite testing DB doesn't support sequences
        if (!this.Database.IsSqlite())
        {
            modelBuilder
                .HasSequence<long>("Viewer_id", schema: "dbo")
                .StartsAt(10000000000L)
                .IncrementsBy(1);

            modelBuilder
                .Entity<DbSavefileUserData>()
                .Property(o => o.ViewerId)
                .HasDefaultValueSql("NEXT VALUE FOR dbo.Viewer_id");
        }

        modelBuilder
            .Entity<DbPlayerCharaData>()
            .HasKey(key => new { key.DeviceAccountId, key.CharaId });

        modelBuilder
            .Entity<DbPlayerDragonData>()
            .HasKey(key => new { key.DeviceAccountId, key.DragonKeyId });

        modelBuilder
            .Entity<DbPlayerDragonReliability>()
            .HasKey(key => new { key.DeviceAccountId, key.DragonId });

        modelBuilder
            .Entity<DbPlayerUnitStory>()
            .HasKey(key => new { key.DeviceAccountId, key.EntityType, key.EntityId, key.StoryId });

        if (_isDevelopment)
            SeedDatabase(modelBuilder);
    }

    private static void SeedDatabase(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DbDeviceAccount>()
            // Hash corresponds to string "password"
            .HasData(new DbDeviceAccount("id", "NMvdakTznEF6khwWcz17i6GTnDA="));

        modelBuilder.Entity<DbSavefileUserData>().HasData(DbSavefileUserDataFactory.Create("id"));
    }

    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

    public DbSet<DbSavefileUserData> SavefileUserData { get; set; } = null!;
    public DbSet<DbPlayerCharaData> PlayerUnitData { get; set; } = null!;
    public DbSet<DbPlayerDragonData> PlayerDragonData { get; set; } = null!;
    public DbSet<DbPlayerDragonReliability> PlayerDragonReliability { get; set; } = null!;
    public DbSet<DbPlayerUnitStory> PlayerUnitStory { get; set; } = null!;
}
