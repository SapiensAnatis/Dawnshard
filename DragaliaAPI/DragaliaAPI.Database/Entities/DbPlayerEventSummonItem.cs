using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId), nameof(ItemId))]
public class DbPlayerEventSummonItem
{
    public long ViewerId { get; set; }

    public int EventId { get; set; }

    public int ItemId { get; set; }

    public int TimesSummoned { get; set; }

    public DbPlayerEventSummonData? EventSummonData { get; set; }
}
