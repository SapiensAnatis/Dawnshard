using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId), nameof(EventId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(EventId), nameof(PassiveId))]
public class DbPlayerEventPassive : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("EventId")]
    public required int EventId { get; set; }

    [Column("PassiveId")]
    public required int PassiveId { get; set; }

    [Column("Progress")]
    public int Progress { get; set; }
}
