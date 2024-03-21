using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Database table for equipped stamps.
/// </summary>
[PrimaryKey(nameof(ViewerId), nameof(Slot))]
public class DbEquippedStamp : DbPlayerData
{
    public int StampId { get; set; }

    public int Slot { get; set; }
}
