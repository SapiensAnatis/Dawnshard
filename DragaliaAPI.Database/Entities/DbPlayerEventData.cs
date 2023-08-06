using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(EventId))]
public class DbPlayerEventData : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("EventId")]
    public required int EventId { get; set; }

    // Used for daily event bonus, event damage reward, etc.
    [Column("CustomEventFlag")]
    public bool CustomEventFlag { get; set; }
}
