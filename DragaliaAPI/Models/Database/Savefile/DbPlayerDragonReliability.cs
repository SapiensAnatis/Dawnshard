using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbPlayerDragonReliability : IDbHasAccountId
{
    [Column("DEVICE_ACCOUNT_ID")]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DRAGON_ID")]
    [Required]
    public long DragonId { get; set; }

    [Column("REL_EXP")]
    [Required]
    public long ReliabilityExp { get; set; }

    [Column("REL_TOTAL_EXP")]
    [Required]
    public long ReliabilityTotalExp { get; set; }

    [Column("LAST_CONTACT_TIME")]
    [Required]
    public long LastContactTime { get; set; }
}
