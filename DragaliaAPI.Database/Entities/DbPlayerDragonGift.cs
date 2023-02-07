using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonGift")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerDragonGift : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey(nameof(Owner))]
    public string DeviceAccountId { get; set; } = null!;

    [Column("DragonGiftId")]
    [Required]
    public DragonGifts DragonGiftId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }
}
