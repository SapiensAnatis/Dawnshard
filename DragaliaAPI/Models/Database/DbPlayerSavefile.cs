using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Models.Database;

public class DbPlayerSavefile
{
    /// <summary>
    /// The device account ID which identifies the owner of this savefile
    /// </summary>
    [Key]
    public string DeviceAccountId { get; set; } = null!;

    /// <summary>
    /// The player's unique ID, i.e. the one that is used to send friend requests.
    /// </summary>
    public long ViewerId { get; set; }
}