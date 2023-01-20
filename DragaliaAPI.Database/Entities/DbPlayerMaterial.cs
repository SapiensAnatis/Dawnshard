using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerMaterial")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerMaterial : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("MaterialId")]
    [Required]
    public Materials MaterialId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }
}
