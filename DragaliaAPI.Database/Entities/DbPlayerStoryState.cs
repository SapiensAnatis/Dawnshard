using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerStoryState")]
public class DbPlayerStoryState : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Column("StoryType")]
    [Required]
    public StoryTypes StoryType { get; set; }

    [Column("StoryId")]
    [Required]
    public int StoryId { get; set; }

    [Column("State")]
    [Required]
    public byte State { get; set; }
}
