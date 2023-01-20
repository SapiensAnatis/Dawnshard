using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

public interface IDbHasAccountId
{
    /// <summary>
    /// The player that owns this information.
    /// </summary>
    public DbPlayer? Owner { get; set; }

    /// <summary>
    /// The device account ID which identifies the owner of this information.
    /// </summary>
    public string DeviceAccountId { get; set; }
}
