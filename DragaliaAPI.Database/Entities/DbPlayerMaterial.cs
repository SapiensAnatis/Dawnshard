using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerMaterial")]
public class DbPlayerMaterial : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("MaterialId")]
    [Required]
    public Materials MaterialId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }
}
