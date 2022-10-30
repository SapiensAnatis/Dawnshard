using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DragaliaAPI.Models.Data;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerStoryState")]
public class DbPlayerStoryState : IDbHasAccountId
{
    [Column("DeviceAccountId")]
    [Required]
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
