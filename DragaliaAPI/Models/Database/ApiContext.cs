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
        // TODO: put this into IEntityTypeConfiguration classes before this method gets huge
        modelBuilder
            .Entity<DbPlayerCharaData>()
            .HasKey(key => new { key.DeviceAccountId, key.CharaId });

        modelBuilder
            .Entity<DbPlayerDragonReliability>()
            .HasKey(key => new { key.DeviceAccountId, key.DragonId });

        modelBuilder
            .Entity<DbPlayerUnitStory>()
            .HasKey(key => new { key.DeviceAccountId, key.EntityType, key.EntityId, key.StoryId });

        modelBuilder.Entity<DbParty>().HasKey(e => new { e.DeviceAccountId, e.PartyNo });

        if (this.Database.IsSqlite())
        {
            // SQLite doesn't support identity columns that aren't primary keys
            modelBuilder
                .Entity<DbPlayerUserData>()
                .Property(x => x.ViewerId)
                .HasDefaultValueSql("last_insert_rowid()");
        }

        /*
        
        Stop players from adding more than 1 character of the same ID to a party,
        but EF Core doesn't like this and the client probably stops you anyway?

        modelBuilder
            .Entity<DbPartyUnit>()
            .HasIndex(e => new { e.Party.PartyNo, e.CharaId })
            .IsUnique();
        
         */
        if (_isDevelopment)
            SeedDatabase(modelBuilder);
    }

    private static void SeedDatabase(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DbDeviceAccount>()
            // Hash corresponds to string "password"
            .HasData(new DbDeviceAccount("id", "NMvdakTznEF6khwWcz17i6GTnDA="));
    }

    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;

    public DbSet<DbPlayerUserData> PlayerUserData { get; set; } = null!;

    public DbSet<DbPlayerCharaData> PlayerCharaData { get; set; } = null!;

    public DbSet<DbPlayerDragonData> PlayerDragonData { get; set; } = null!;

    public DbSet<DbPlayerDragonReliability> PlayerDragonReliability { get; set; } = null!;

    public DbSet<DbPlayerUnitStory> PlayerUnitStory { get; set; } = null!;

    public DbSet<DbParty> PlayerParties { get; set; } = null!;

    public DbSet<DbPartyUnit> PlayerPartyUnits { get; set; } = null!;
}
