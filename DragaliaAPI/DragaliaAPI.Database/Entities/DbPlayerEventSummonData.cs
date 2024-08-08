using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId))]
public class DbPlayerEventSummonData : DbPlayerData
{
    public int EventId { get; set; }

    public int Points { get; set; }

    public int BoxNumber { get; set; } = 1;

    public List<DbPlayerEventSummonItem> Items { get; set; } = [];
}
