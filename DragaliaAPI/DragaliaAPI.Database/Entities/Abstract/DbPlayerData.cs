using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities.Abstract;

public abstract class DbPlayerData : IDbPlayerData
{
    /// <summary>
    /// The player that owns this information.
    /// </summary>
    public DbPlayer? Owner { get; set; }

    /// <summary>
    /// The viewer ID which identifies the owner of this information.
    /// </summary>
    [ForeignKey(nameof(Owner))]
    public long ViewerId { get; set; }
}
