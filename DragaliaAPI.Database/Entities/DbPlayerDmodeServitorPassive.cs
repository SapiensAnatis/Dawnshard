using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(PassiveId))]
public class DbPlayerDmodeServitorPassive : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("PassiveId")]
    public required DmodeServitorPassiveType PassiveId { get; set; }

    [Column("Level")]
    public int Level { get; set; } = 1;
}
