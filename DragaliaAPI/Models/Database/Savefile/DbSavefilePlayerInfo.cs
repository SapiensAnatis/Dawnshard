using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbSavefilePlayerInfo : IDbSavefile
{
    /// <inheritdoc/>
    [Key]
    [Required, ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    /// <summary>
    /// The player's unique ID, i.e. the one that is used to send friend requests.
    /// </summary>
    [Key]
    [Required]
    public long ViewerId { get; set; }

    /// <summary>
    /// The player's display name.
    /// </summary>
    [Required]
    [DefaultValue("Euden")]
    public string Name { get; set; } = null!;

    [Required]
    [DefaultValue(1)]
    public int Level { get; set; }

    [Required]
    [DefaultValue(0)]
    public int Exp { get; set; }

    /// <summary>
    /// The player's wyrmite balance.
    /// </summary>
    [Required]
    [DefaultValue(0)]
    public int Crystal { get; set; }

    /// <summary>
    /// The player's rupie balance.
    /// </summary>
    [Required]
    [DefaultValue(0)]
    public int Coin { get; set; }

    [Required]
    [DefaultValue(160)]
    public int MaxDragonQuantity { get; set; }

    [Required]
    [DefaultValue(0)]
    public int MaxWeaponQuantity { get; set; }

    [Required]
    [DefaultValue(0)]
    public int MaxAmuletQuantity { get; set; }

    [Required]
    [DefaultValue(0)]
    public int QuestSkipPoint { get; set; }

    [Required]
    [DefaultValue(1)]
    public int MainPartyNo { get; set; }

    [Required]
    [DefaultValue(40000001)]
    public int EmblemId { get; set; }

    [Required]
    [DefaultValue(0)]
    public int ActiveMemoryEventId { get; set; }

    [Required]
    [DefaultValue(0)]
    public int ManaPoint { get; set; }

    [Required]
    [DefaultValue(0)]
    public int DewPoint { get; set; }

    [Required]
    [DefaultValue(0)]
    public int BuildTimePoint { get; set; }

    [Required]
    [DefaultValue(0)]
    public int LastLoginTime { get; set; } // Year 2038 problem!

    [Required]
    [DefaultValue(18)]
    public int StaminaSingle { get; set; }

    [Required]
    [DefaultValue(0)]
    public int LastStaminaSingleUpdateTime { get; set; }

    [Required]
    [DefaultValue(0)]
    public int StaminaSingleSurplusSecond { get; set; }

    [Required]
    [DefaultValue(0)]
    public int StaminaMulti { get; set; }

    [Required]
    [DefaultValue(0)]
    public int LastStaminaMultiUpdateTime { get; set; }

    [Required]
    [DefaultValue(0)]
    public int StaminaMultiSurplusSecond { get; set; }

    [Required]
    [DefaultValue(0)]
    public int TutorialStatus { get; set; }

    [Required]
    [DefaultValue(0)]
    public int PrologueEndTime { get; set; }

    [Required]
    [DefaultValue(0)]
    public int IsOptin { get; set; }

    [Required]
    [DefaultValue(0)]
    public int FortOpenTime { get; set; }

    [Required]
    [DefaultValue(0)]
    public int CreateTime { get; set; }
}