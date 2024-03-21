using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database;

/// <summary>
/// Base database context.
/// </summary>
public class ApiContext : DbContext, IDataProtectionKeyContext
{
    private readonly IPlayerIdentityService playerIdentityService;

    public ApiContext(
        DbContextOptions<ApiContext> options,
        IPlayerIdentityService playerIdentityService
    )
        : base(options)
    {
        this.playerIdentityService = playerIdentityService;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public DbSet<DbDeviceAccount> DeviceAccounts { get; set; } = null!;
#pragma warning restore CS0618 // Type or member is obsolete

    public DbSet<DbPlayer> Players { get; set; } = null!;

    public DbSet<DbPlayerUserData> PlayerUserData { get; set; } = null!;

    public DbSet<DbPlayerCharaData> PlayerCharaData { get; set; } = null!;

    public DbSet<DbPlayerDragonData> PlayerDragonData { get; set; } = null!;

    public DbSet<DbPlayerDragonReliability> PlayerDragonReliability { get; set; } = null!;

    public DbSet<DbPlayerStoryState> PlayerStoryState { get; set; } = null!;

    public DbSet<DbQuest> PlayerQuests { get; set; } = null!;

    public DbSet<DbPlayerSummonHistory> PlayerSummonHistory { get; set; } = null!;

    public DbSet<DbPlayerBannerData> PlayerBannerData { get; set; } = null!;

    public DbSet<DbParty> PlayerParties { get; set; } = null!;

    public DbSet<DbPartyUnit> PlayerPartyUnits { get; set; } = null!;

    public DbSet<DbPlayerMaterial> PlayerMaterials { get; set; } = null!;

    public DbSet<DbSetUnit> PlayerSetUnits { get; set; } = null!;

    public DbSet<DbAbilityCrest> PlayerAbilityCrests { get; set; } = null!;

    public DbSet<DbAbilityCrestSet> PlayerAbilityCrestSets { get; set; } = null!;

    public DbSet<DbWeaponBody> PlayerWeapons { get; set; } = null!;

    public DbSet<DbTalisman> PlayerTalismans { get; set; } = null!;

    public DbSet<DbFortBuild> PlayerFortBuilds { get; set; } = null!;

    public DbSet<DbFortDetail> PlayerFortDetails { get; set; } = null!;

    public DbSet<DbWeaponSkin> PlayerWeaponSkins { get; set; } = null!;

    public DbSet<DbWeaponPassiveAbility> PlayerPassiveAbilities { get; set; } = null!;

    public DbSet<DbPlayerDragonGift> PlayerDragonGifts { get; set; } = null!;

    public DbSet<DbPlayerMission> PlayerMissions { get; set; } = null!;

    public DbSet<DbCompletedDailyMission> CompletedDailyMissions { get; set; } = null!;

    public DbSet<DbPlayerPresent> PlayerPresents { get; set; } = null!;

    public DbSet<DbPlayerPresentHistory> PlayerPresentHistory { get; set; } = null!;

    public DbSet<DbEquippedStamp> EquippedStamps { get; set; } = null!;

    public DbSet<DbPlayerShopInfo> PlayerShopInfos { get; set; } = null!;

    public DbSet<DbPlayerShopPurchase> PlayerPurchases { get; set; } = null!;

    public DbSet<DbPlayerTrade> PlayerTrades { get; set; } = null!;

    public DbSet<DbPlayerEventData> PlayerEventData { get; set; } = null!;

    public DbSet<DbPlayerEventItem> PlayerEventItems { get; set; } = null!;

    public DbSet<DbPlayerEventReward> PlayerEventRewards { get; set; } = null!;

    public DbSet<DbPlayerEventPassive> PlayerEventPassives { get; set; } = null!;

    public DbSet<DbQuestClearPartyUnit> QuestClearPartyUnits { get; set; } = null!;

    public DbSet<DbPlayerDmodeInfo> PlayerDmodeInfos { get; set; } = null!;

    public DbSet<DbPlayerDmodeChara> PlayerDmodeCharas { get; set; } = null!;

    public DbSet<DbPlayerDmodeDungeon> PlayerDmodeDungeons { get; set; } = null!;

    public DbSet<DbPlayerDmodeServitorPassive> PlayerDmodeServitorPassives { get; set; } = null!;

    public DbSet<DbPlayerDmodeExpedition> PlayerDmodeExpeditions { get; set; } = null!;

    public DbSet<DbPlayerUseItem> PlayerUseItems { get; set; } = null!;

    public DbSet<DbLoginBonus> LoginBonuses { get; set; } = null!;

    public DbSet<DbSummonTicket> PlayerSummonTickets { get; set; } = null!;

    public DbSet<DbEmblem> Emblems { get; set; } = null!;

    public DbSet<DbQuestEvent> QuestEvents { get; set; } = null!;

    public DbSet<DbPartyPower> PartyPowers { get; set; } = null!;

    public DbSet<DbTimeAttackClear> TimeAttackClears { get; set; } = null!;

    public DbSet<DbTimeAttackPlayer> TimeAttackPlayers { get; set; } = null!;

    public DbSet<DbTimeAttackClearUnit> TimeAttackClearUnits { get; set; } = null!;

    public DbSet<DbReceivedRankingTierReward> ReceivedRankingTierRewards { get; set; } = null!;

    public DbSet<DbNewsItem> NewsItems { get; set; } = null!;

    public DbSet<DbQuestTreasureList> QuestTreasureList { get; set; } = null!;

    public DbSet<DbPlayerQuestWall> PlayerQuestWalls { get; set; } = null!;

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DbSummonTicket>()
            .HasQueryFilter(x => x.ViewerId == this.playerIdentityService.ViewerId);

        modelBuilder
            .Entity<DbPlayerStoryState>()
            .HasQueryFilter(x => x.ViewerId == this.playerIdentityService.ViewerId);

        modelBuilder
            .Entity<DbPlayerDragonGift>()
            .HasQueryFilter(x => x.ViewerId == this.playerIdentityService.ViewerId);
    }
}
