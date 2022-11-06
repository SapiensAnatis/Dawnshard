using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Models.Data.Entity;
using Microsoft.Build.Framework;

namespace DragaliaAPI.Models.Database.Savefile;

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
