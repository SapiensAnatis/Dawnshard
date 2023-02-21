using System.ComponentModel.DataAnnotations;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Container class for all savefile data to enable foreign keys.
/// </summary>
public class DbPlayer
{
    [Key]
    public required string AccountId { get; set; }

    public virtual DbPlayerUserData? UserData { get; set; }

    public virtual ICollection<DbAbilityCrest> AbilityCrestList { get; set; } =
        new List<DbAbilityCrest>();

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

    public virtual ICollection<DbPlayerCurrency> Currencies { get; set; } =
        new List<DbPlayerCurrency>();

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
}
