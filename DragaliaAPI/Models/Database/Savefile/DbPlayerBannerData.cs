using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerBannerData")]
public class DbPlayerBannerData : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("SummonBannerId")]
    [Required]
    public int SummonBannerId { get; set; }

    [Column("Pity")]
    [Required]
    public byte PityRate { get; set; }

    [Column("SummonCount")]
    [Required]
    public int SummonCount { get; set; }

    [Column("DailyLimitedSummons")]
    [Required]
    public int DailyLimitedSummonCount { get; set; }

    [Column("FreeSummonAvailable")]
    [Required]
    public int IsFreeSummonAvailable { get; set; }

    [Column("BeginnerSummonAvailable")]
    [Required]
    public int IsBeginnerFreeSummonAvailable { get; set; }

    [Column("CsSummonAvailable")]
    [Required]
    public int IsConsecutionFreeSummonAvailable { get; set; }

    [Column("SummonPoints")]
    [Required]
    public int SummonPoints { get; set; }

    [Column("CsSummonPoints")]
    [Required]
    public int ConsecutionSummonPoints { get; set; }

    //TODO Not sure if these two belong here
    [Column("CsSummonPointsMinDate")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ConsecutionSummonPointsMinDate { get; set; }

    [Column("CsSummonPointsMaxDate")]
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ConsecutionSummonPointsMaxDate { get; set; }
}
