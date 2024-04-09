using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Container class for all savefile data to enable foreign keys.
/// </summary>
[Index(nameof(AccountId))]
public class DbPlayer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ViewerId { get; set; }

    [MaxLength(16)]
    public required string AccountId { get; set; }

    public int SavefileVersion { get; set; }

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
}
