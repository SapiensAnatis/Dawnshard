using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId))]
public class DbPlayerEventData : DbPlayerData
{
    [Column("EventId")]
    public required int EventId { get; set; }

    // Used for daily event bonus, event damage reward, etc.
    [Column("CustomEventFlag")]
    public bool CustomEventFlag { get; set; }
}
