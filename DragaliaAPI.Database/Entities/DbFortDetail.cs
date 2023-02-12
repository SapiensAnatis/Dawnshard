using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerFortDetail")]
[Index(nameof(DeviceAccountId))]
public class DbFortDetail : IDbHasAccountId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long FortId { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("CarpenterNum")]
    [Required]
    public int CarpenterNum { get; set; }

    [Column("MaxCarpenterCount")]
    [Required]
    public int MaxCarpenterCount { get; set; }

    [Column("WorkingCarpenterNum")]
    [Required]
    public int WorkingCarpenterNum { get; set; }
}
