using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerCharaHonor")]
[PrimaryKey(nameof(ViewerId), nameof(CharaId), nameof(HonorId))]
public class DbPlayerCharaHonor : DbPlayerData
{
    [Column("CharaId")]
    public Charas CharaId { get; set; }

    [Column("HonorId")]
    public int HonorId { get; set; }
}
