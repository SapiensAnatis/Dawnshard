using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbPlayerUnitStory : IDbHasAccountId
{
    [Column("DEVICE_ACCOUNT_ID")]
    [Required]
    public string DeviceAccountId { get; set; } = null!;

    [Column("ENTITY_TYPE")]
    [Required]
    public byte EntityType { get; set; }

    [Column("ENTITY_ID")]
    [Required]
    public long EntityId { get; set; }

    [Column("STORY_ID")]
    [Required]
    public long StoryId { get; set; }

    [Column("DRAGON_ID")]
    [Required]
    [TypeConverter(typeof(BooleanConverter))]
    public bool DragonId { get; set; }
}
