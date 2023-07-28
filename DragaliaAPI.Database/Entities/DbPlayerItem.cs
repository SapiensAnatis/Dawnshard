using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(ItemId))]
public class DbPlayerUseItem : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("ItemId")]
    public required UseItem ItemId { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }
}
