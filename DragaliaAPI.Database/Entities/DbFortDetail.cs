using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerFortDetail")]
[Index(nameof(DeviceAccountId))]
public class DbFortDetail : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [Key]
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("CarpenterNum")]
    [Required]
    public int CarpenterNum { get; set; }
}
