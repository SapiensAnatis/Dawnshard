using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerCurrency")]
public class DbPlayerCurrency : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("CurrencyType")]
    [Required]
    public CurrencyTypes CurrencyType { get; set; }

    [Column("Quantity")]
    [Required]
    public long Quantity { get; set; }
}
