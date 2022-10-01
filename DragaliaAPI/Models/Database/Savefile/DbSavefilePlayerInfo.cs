using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbSavefilePlayerInfo : IDbSavefile
{
    /// <inheritdoc/>
    [Key]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    /// <summary>
    /// The player's unique ID, i.e. the one that is used to send friend requests.
    /// </summary>
    [Required]
    public long ViewerId { get; set; }

    /// <summary>
    /// The player's display name.
    /// </summary>
    [Required]
    [DefaultValue("Euden")]
    public string Name { get; set; } = null!;
}