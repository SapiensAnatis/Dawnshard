using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(EmblemId))]
public class DbEmblem : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("EmblemId")]
    public required Emblems EmblemId { get; set; }

    [Column("IsNew")]
    public bool IsNew { get; set; }

    [Column("GetTime")]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UnixEpoch;
}
