using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerCurrency")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerCurrency : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("CurrencyType")]
    [Required]
    public CurrencyTypes CurrencyType { get; set; }

    [Column("Quantity")]
    [Required]
    public long Quantity { get; set; }
}
