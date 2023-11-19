using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerMaterial")]
[PrimaryKey(nameof(ViewerId), nameof(MaterialId))]
public class DbPlayerMaterial : DbPlayerData
{
    [Column("MaterialId")]
    [Required]
    public Materials MaterialId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }
}
