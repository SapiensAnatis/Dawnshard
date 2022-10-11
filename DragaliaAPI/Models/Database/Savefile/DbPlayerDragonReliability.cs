using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerDragonReliability")]
public class DbPlayerDragonReliability : IDbHasAccountId
{
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    [TypeConverter(typeof(EnumConverter))]
    public Dragons DragonId { get; set; }

    [Required]
    public long ReliabilityExp { get; set; }

    [Required]
    public long ReliabilityTotalExp { get; set; }

    [Required]
    public long LastContactTime { get; set; }
}
