using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerFortDetail")]
[PrimaryKey(nameof(ViewerId))]
public class DbFortDetail : DbPlayerData
{
    [Column("CarpenterNum")]
    [Required]
    public int CarpenterNum { get; set; }
}
