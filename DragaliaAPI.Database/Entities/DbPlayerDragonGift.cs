using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerDragonGift")]
[PrimaryKey(nameof(ViewerId), nameof(DragonGiftId))]
public class DbPlayerDragonGift : DbPlayerData
{
    [Column("DragonGiftId")]
    [Required]
    public DragonGifts DragonGiftId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }
}
