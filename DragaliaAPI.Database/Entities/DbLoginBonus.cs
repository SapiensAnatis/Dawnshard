using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId), nameof(Id))]
public class DbLoginBonus : IDbHasAccountId
{
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the login bonus.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the most recent day earned for this login bonus.
    /// </summary>
    public int CurrentDay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the login bonus has been completed.
    /// </summary>
    public bool IsComplete { get; set; }
}
