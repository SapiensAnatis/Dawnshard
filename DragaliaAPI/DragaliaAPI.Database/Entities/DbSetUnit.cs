using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerSetUnit")]
[PrimaryKey(nameof(ViewerId), nameof(CharaId), nameof(UnitSetNo))]
public class DbSetUnit : DbUnitBase, IDbPlayerData
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public long ViewerId { get; set; }

    [Required]
    public int UnitSetNo { get; set; }

    [StringLength(32)]
    public required string UnitSetName { get; set; }
}
