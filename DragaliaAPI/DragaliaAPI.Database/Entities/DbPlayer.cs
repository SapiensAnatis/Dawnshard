using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Container class for all savefile data to enable foreign keys.
/// </summary>
public class DbPlayer
{
    public long ViewerId { get; set; }

    public required string AccountId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public int SavefileVersion { get; set; }

    /// <summary>
    /// The last time at which a savefile for this user was imported from BaaS.
    /// </summary>
    public DateTimeOffset? LastSavefileImportTime { get; set; }

    public string? SavefileOrigin { get; set; }

    public bool IsAdmin { get; set; }

    public DbPlayerUserData? UserData { get; set; }

    public List<DbAbilityCrest> AbilityCrestList { get; set; } = [];

    public List<DbAbilityCrestSet> AbilityCrestSetList { get; set; } = [];

    public List<DbFortBuild> BuildList { get; set; } = [];

    public List<DbParty> PartyList { get; set; } = [];

    public List<DbPlayerBannerData> BannerData { get; set; } = [];

    public List<DbPlayerCharaData> CharaList { get; set; } = [];

    public List<DbPlayerDragonData> DragonList { get; set; } = [];

    public List<DbPlayerDragonGift> DragonGiftList { get; set; } = [];

    public List<DbPlayerDragonReliability> DragonReliabilityList { get; set; } = [];

    public List<DbPlayerMaterial> MaterialList { get; set; } = [];

    public List<DbSetUnit> UnitSets { get; set; } = [];

    public List<DbPlayerStoryState> StoryStates { get; set; } = [];

    public List<DbPlayerSummonHistory> SummonHistory { get; set; } = [];

    public List<DbQuest> QuestList { get; set; } = [];

    public List<DbWeaponBody> WeaponBodyList { get; set; } = [];

    public List<DbTalisman> TalismanList { get; set; } = [];

    public List<DbWeaponSkin> WeaponSkinList { get; set; } = [];

    public List<DbWeaponPassiveAbility> WeaponPassiveAbilityList { get; set; } = [];

    public DbFortDetail? FortDetail { get; set; }

    public List<DbEquippedStamp> EquippedStampList { get; set; } = [];

    public List<DbPlayerPresent> Presents { get; set; } = [];

    public List<DbPlayerPresentHistory> PresentHistory { get; set; } = [];

    public List<DbPlayerDmodeChara> DmodeCharas { get; set; } = [];

    public DbPlayerDmodeDungeon? DmodeDungeon { get; set; }

    public DbPlayerDmodeExpedition? DmodeExpedition { get; set; }

    public DbPlayerDmodeInfo? DmodeInfo { get; set; }

    public List<DbPlayerDmodeServitorPassive> DmodeServitorPassives { get; set; } = [];

    public DbPlayerShopInfo? ShopInfo { get; set; }

    public List<DbQuestEvent> QuestEvents { get; set; } = [];

    public DbPartyPower? PartyPower { get; set; }

    public List<DbQuestTreasureList> QuestTreasureList { get; set; } = [];

    public List<DbPlayerQuestWall> QuestWalls { get; set; } = [];

    public List<DbPlayerTrade> Trades { get; set; } = [];

    public List<DbSummonTicket> SummonTickets { get; set; } = [];

    public List<DbEmblem> Emblems { get; set; } = [];

    public DbWallRewardDate? WallRewardDate { get; set; }

    public DbPlayerDiamondData? DiamondData { get; set; }

    public DbPlayerHelper? Helper { get; set; }

    public List<DbPlayerFriendship> Friendships { get; set; } = [];

    public DbSettings? Settings { get; set; }

    private class Configuration : IEntityTypeConfiguration<DbPlayer>
    {
        public void Configure(EntityTypeBuilder<DbPlayer> builder)
        {
            builder.HasKey(e => e.ViewerId);

            builder.HasIndex(e => e.AccountId);
            builder.Property(e => e.AccountId).HasMaxLength(16);

            builder.Property(e => e.SavefileOrigin).HasMaxLength(32);
        }
    }
}
