using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbSavefileUserData : IDbHasAccountId
{
    /// <inheritdoc/>
    [Key]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    /// <summary>
    /// The player's unique ID, i.e. the one that is used to send friend requests.
    /// </summary>
    public long ViewerId { get; set; }

    /// <summary>
    /// The player's display name.
    /// </summary>
    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public int Exp { get; set; }

    /// <summary>
    /// The player's wyrmite balance.
    /// </summary>
    public int Crystal { get; set; }

    /// <summary>
    /// The player's rupie balance.
    /// </summary>
    public int Coin { get; set; }

    public int MaxDragonQuantity { get; set; }

    public int MaxWeaponQuantity { get; set; }

    public int MaxAmuletQuantity { get; set; }

    public int QuestSkipPoint { get; set; }

    public int MainPartyNo { get; set; }

    public int EmblemId { get; set; }

    public int ActiveMemoryEventId { get; set; }

    public int ManaPoint { get; set; }

    public int DewPoint { get; set; }

    public int BuildTimePoint { get; set; }

    public int LastLoginTime { get; set; } // Year 2038 problem!

    public int StaminaSingle { get; set; }

    public int LastStaminaSingleUpdateTime { get; set; }

    public int StaminaSingleSurplusSecond { get; set; }

    public int StaminaMulti { get; set; }

    public int LastStaminaMultiUpdateTime { get; set; }

    public int StaminaMultiSurplusSecond { get; set; }

    public int TutorialStatus { get; set; }

    public int TutorialFlag { get; set; }

    public int PrologueEndTime { get; set; }

    public int IsOptin { get; set; }

    public int FortOpenTime { get; set; }

    public int CreateTime { get; set; }
}

public static class DbSavefileUserDataFactory
{
    public static DbSavefileUserData Create(string deviceAccountId)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            Name = "Euden",
            Level = 1,
            MaxDragonQuantity = 160,
            MainPartyNo = 1,
            EmblemId = 40000001,
            StaminaSingle = 18,
            StaminaMulti = 12,
            // Matches internal datatype of the game -- I guess they anticipated EOS before 2038
            CreateTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
    }
}
