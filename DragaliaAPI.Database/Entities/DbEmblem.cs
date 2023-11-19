using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EmblemId))]
public class DbEmblem : DbPlayerData
{
    [Column("EmblemId")]
    public required Emblems EmblemId { get; set; }

    [Column("IsNew")]
    public bool IsNew { get; set; }

    [Column("GetTime")]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UnixEpoch;
}
