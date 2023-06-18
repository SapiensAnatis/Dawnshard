using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Database table for equipped stamps.
/// </summary>
[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(Slot))]
public class DbEquippedStamp : IDbHasAccountId
{
    public DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public int StampId { get; set; }

    public int Slot { get; set; }
}
