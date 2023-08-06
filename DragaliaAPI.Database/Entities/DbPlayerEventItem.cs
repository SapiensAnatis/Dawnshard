using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId), nameof(EventId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(Id))]
public class DbPlayerEventItem : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("Id")]
    public required int Id { get; set; }

    [Column("Type")]
    public required int Type { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }
}
