using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(ViewerId), nameof(EventId))]
[PrimaryKey(nameof(ViewerId), nameof(Id))]
public class DbPlayerEventItem : DbPlayerData
{
    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("Id")]
    public required int Id { get; set; }

    [Column("Type")]
    public required int Type { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }
}
