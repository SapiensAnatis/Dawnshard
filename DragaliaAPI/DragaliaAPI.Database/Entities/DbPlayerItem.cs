using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(ItemId))]
public class DbPlayerUseItem : DbPlayerData
{
    [Column("ItemId")]
    public required UseItem ItemId { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }
}
