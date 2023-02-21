using System.Reflection;
using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database;

/// <summary>
/// Base database context.
/// </summary>
/// <remarks>Do not use this class directly -- make a repository method instead. This rule is enforced to make queries easy to unit test.</remarks>
public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options) { }

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
            .Entity<DbPlayerCurrency>()
            .HasKey(key => new { key.DeviceAccountId, key.CurrencyType });

        modelBuilder
            .Entity<DbPlayerDragonGift>()
            .HasKey(key => new { key.DeviceAccountId, key.DragonGiftId });

        modelBuilder
            .Entity<DbPlayerMaterial>()
            .HasKey(key => new { key.DeviceAccountId, key.MaterialId });

        modelBuilder
            .Entity<DbPlayerStoryState>()
            .HasKey(
                key =>
                    new
                    {
                        key.DeviceAccountId,
                        key.StoryType,
                        key.StoryId
                    }
            );

        modelBuilder.Entity<DbParty>().HasKey(e => new { e.DeviceAccountId, e.PartyNo });

        modelBuilder
            .Entity<DbSetUnit>()
            .HasKey(
                key =>
                    new
                    {
                        key.DeviceAccountId,
                        key.CharaId,
                        key.UnitSetNo
                    }
            );

        modelBuilder
            .Entity<DbPlayerBannerData>()
            .HasKey(key => new { key.DeviceAccountId, key.SummonBannerId });

        modelBuilder.Entity<DbQuest>().HasKey(e => new { e.DeviceAccountId, e.QuestId });

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; }
#pragma warning restore CS0618 // Type or member is obsolete

    public DbSet<DbPlayer> Players { get; set; }

    public DbSet<DbPlayerUserData> PlayerUserData { get; set; }

    public DbSet<DbPlayerCharaData> PlayerCharaData { get; set; }

    public DbSet<DbPlayerDragonData> PlayerDragonData { get; set; }

    public DbSet<DbPlayerDragonReliability> PlayerDragonReliability { get; set; }

    public DbSet<DbPlayerStoryState> PlayerStoryState { get; set; }

    public DbSet<DbQuest> PlayerQuests { get; set; }

    public DbSet<DbPlayerSummonHistory> PlayerSummonHistory { get; set; }

    public DbSet<DbPlayerBannerData> PlayerBannerData { get; set; }

    public DbSet<DbParty> PlayerParties { get; set; }

    public DbSet<DbPartyUnit> PlayerPartyUnits { get; set; }

    public DbSet<DbPlayerCurrency> PlayerWallet { get; set; }

    public DbSet<DbPlayerMaterial> PlayerMaterials { get; set; }

    public DbSet<DbSetUnit> PlayerSetUnits { get; set; }

    public DbSet<DbAbilityCrest> PlayerAbilityCrests { get; set; }

    public DbSet<DbWeaponBody> PlayerWeapons { get; set; }

    public DbSet<DbTalisman> PlayerTalismans { get; set; }

    public DbSet<DbFortBuild> PlayerFortBuilds { get; set; }

    public DbSet<DbWeaponSkin> PlayerWeaponSkins { get; set; }

    public DbSet<DbWeaponPassiveAbility> PlayerPassiveAbilities { get; set; }

    public DbSet<DbPlayerDragonGift> PlayerDragonGifts { get; set; }
}
