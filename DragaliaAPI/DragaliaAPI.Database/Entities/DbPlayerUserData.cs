using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerUserData")]
[PrimaryKey(nameof(ViewerId))]
public class DbPlayerUserData : DbPlayerData
{
    /// <summary>
    /// The player's display name.
    /// </summary>
    [MaxLength(10)]
    public string Name { get; set; } = "Euden";

    public int Level { get; set; } = 1;

    public int Exp { get; set; }

    /// <summary>
    /// The player's wyrmite balance.
    /// </summary>
    public int Crystal { get; set; }

    /// <summary>
    /// The player's rupie balance.
    /// </summary>
    public long Coin { get; set; }

    public int MaxDragonQuantity { get; set; } = 200;

    public int QuestSkipPoint { get; set; }

    public int MainPartyNo { get; set; } = 1;

    public Emblems EmblemId { get; set; } = Emblems.DragonbloodPrince;

    public int ActiveMemoryEventId { get; set; }

    public int ManaPoint { get; set; }

    public int DewPoint { get; set; }

    public int BuildTimePoint { get; set; }

    public DateTimeOffset LastLoginTime { get; set; } = DateTimeOffset.UnixEpoch;

    public int StaminaSingle { get; set; } = 18;

    public DateTimeOffset LastStaminaSingleUpdateTime { get; set; } = DateTimeOffset.UtcNow;

    public int StaminaSingleSurplusSecond { get; set; }

    public int StaminaMulti { get; set; } = 12;

    public DateTimeOffset LastStaminaMultiUpdateTime { get; set; } = DateTimeOffset.UtcNow;

    public int StaminaMultiSurplusSecond { get; set; }

    public int TutorialStatus { get; set; }

    public int TutorialFlag { get; set; }

    [NotMapped]
    public ISet<int> TutorialFlagList
    {
        get => TutorialFlagUtil.ConvertIntToFlagIntList(TutorialFlag);
        set => TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(value);
    }

    public DateTimeOffset FortOpenTime { get; set; } = DateTimeOffset.UnixEpoch;

    public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The last time at which a savefile for this user was imported from BaaS.
    /// </summary>
    public DateTimeOffset LastSaveImportTime { get; set; }
}
