using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DragaliaAPI.Models.Database.Savefile;

[Table("PlayerUnitStory")]
public class DbPlayerUnitStory : IDbHasAccountId
{
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    public byte EntityType { get; set; }

    [Required]
    public long EntityId { get; set; }

    [Required]
    public long StoryId { get; set; }

    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool DragonId { get; set; }
}
