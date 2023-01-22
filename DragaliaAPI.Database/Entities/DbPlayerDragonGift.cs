using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonGift")]
public class DbPlayerDragonGift : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DragonGiftId")]
    [Required]
    public DragonGifts DragonGiftId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }
}
