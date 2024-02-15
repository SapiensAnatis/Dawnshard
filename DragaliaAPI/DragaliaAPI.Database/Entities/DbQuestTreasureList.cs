using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(QuestTreasureId))]
public class DbQuestTreasureList : DbPlayerData
{
    [Column("QuestTreasureId")]
    public required int QuestTreasureId { get; set; }
}
