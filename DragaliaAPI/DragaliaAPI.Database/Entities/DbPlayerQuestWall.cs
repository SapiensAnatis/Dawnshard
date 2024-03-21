using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(WallId))]
public class DbPlayerQuestWall : DbPlayerData
{
    public required int WallId { get; set; }

    public int WallLevel { get; set; }

    public bool IsStartNextLevel { get; set; }
}
