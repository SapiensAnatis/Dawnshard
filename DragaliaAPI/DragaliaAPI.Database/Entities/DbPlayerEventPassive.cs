using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId), nameof(PassiveId))]
public class DbPlayerEventPassive : DbPlayerData
{
    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("PassiveId")]
    public required int PassiveId { get; set; }

    [Column("Progress")]
    public int Progress { get; set; }
}
