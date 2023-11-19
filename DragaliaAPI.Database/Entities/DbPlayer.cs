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

    public virtual DbPlayerUserData? UserData { get; set; }

    public virtual ICollection<DbAbilityCrest> AbilityCrestList { get; set; } =
        new List<DbAbilityCrest>();

    public virtual ICollection<DbAbilityCrestSet> AbilityCrestSetList { get; set; } =
        new List<DbAbilityCrestSet>();

    public virtual ICollection<DbFortBuild> BuildList { get; set; } = new List<DbFortBuild>();

    public virtual ICollection<DbParty> PartyList { get; set; } = new List<DbParty>();

    public virtual ICollection<DbPlayerBannerData> UserSummonList { get; set; } =
        new List<DbPlayerBannerData>();

    public virtual ICollection<DbPlayerCharaData> CharaList { get; set; } =
        new List<DbPlayerCharaData>();

    public virtual ICollection<DbPlayerDragonData> DragonList { get; set; } =
        new List<DbPlayerDragonData>();

    public virtual ICollection<DbPlayerDragonGift> DragonGiftList { get; set; } =
        new List<DbPlayerDragonGift>();

    public virtual ICollection<DbPlayerDragonReliability> DragonReliabilityList { get; set; } =
        new List<DbPlayerDragonReliability>();

    public virtual ICollection<DbPlayerMaterial> MaterialList { get; set; } =
        new List<DbPlayerMaterial>();

    public virtual ICollection<DbSetUnit> UnitSets { get; set; } = new List<DbSetUnit>();

    public virtual ICollection<DbPlayerStoryState> StoryStates { get; set; } =
        new List<DbPlayerStoryState>();

    public virtual ICollection<DbPlayerSummonHistory> SummonHistory { get; set; } =
        new List<DbPlayerSummonHistory>();

    public virtual ICollection<DbQuest> QuestList { get; set; } = new List<DbQuest>();

    public virtual ICollection<DbWeaponBody> WeaponBodyList { get; set; } =
        new List<DbWeaponBody>();

    public virtual ICollection<DbTalisman> TalismanList { get; set; } = new List<DbTalisman>();

    public virtual ICollection<DbWeaponSkin> WeaponSkinList { get; set; } =
        new List<DbWeaponSkin>();

    public virtual ICollection<DbWeaponPassiveAbility> WeaponPassiveAbilityList { get; set; } =
        new List<DbWeaponPassiveAbility>();

    public virtual DbFortDetail? FortDetail { get; set; }

    public virtual ICollection<DbEquippedStamp> EquippedStampList { get; set; } =
        new List<DbEquippedStamp>();

    public virtual ICollection<DbPlayerPresent> Presents { get; set; } =
        new List<DbPlayerPresent>();

    public virtual ICollection<DbPlayerPresentHistory> PresentHistory { get; set; } =
        new List<DbPlayerPresentHistory>();

    public virtual ICollection<DbPlayerDmodeChara> DmodeCharas { get; set; } =
        new List<DbPlayerDmodeChara>();

    public virtual DbPlayerDmodeDungeon? DmodeDungeon { get; set; }

    public virtual DbPlayerDmodeExpedition? DmodeExpedition { get; set; }

    public virtual DbPlayerDmodeInfo? DmodeInfo { get; set; }

    public virtual ICollection<DbPlayerDmodeServitorPassive> DmodeServitorPassives { get; set; } =
        new List<DbPlayerDmodeServitorPassive>();

    public virtual DbPlayerShopInfo? ShopInfo { get; set; }

    public virtual ICollection<DbQuestEvent> QuestEvents { get; set; } = new List<DbQuestEvent>();

    public virtual DbPartyPower? PartyPower { get; set; }

    public virtual ICollection<DbQuestTreasureList> QuestTreasureList { get; set; } =
        new List<DbQuestTreasureList>();

    public virtual ICollection<DbPlayerQuestWall> QuestWalls { get; set; } =
        new List<DbPlayerQuestWall>();

    public virtual ICollection<DbPlayerTrade> Trades { get; set; } = new List<DbPlayerTrade>();

    public virtual ICollection<DbSummonTicket> SummonTickets { get; set; } =
        new List<DbSummonTicket>();

    public virtual ICollection<DbEmblem> Emblems { get; set; } = new List<DbEmblem>();
}
