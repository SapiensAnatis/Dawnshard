using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerGatherItem")]
[PrimaryKey(nameof(ViewerId), nameof(GatherItemId))]
public class DbPlayerGatherItem : DbPlayerData
{
    [Column("GatherItemId")]
    [Required]
    public int GatherItemId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }

    [Column("QuestTakeWeeklyQuantity")]
    [Required]
    public int QuestTakeWeeklyQuantity { get; set; }

    [Column("QuestLastWeeklyResetTime")]
    [Required]
    public DateTimeOffset QuestLastWeeklyResetTime { get; set; }
}
