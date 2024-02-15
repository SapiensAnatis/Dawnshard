using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerStoryState")]
[PrimaryKey(nameof(ViewerId), nameof(StoryType), nameof(StoryId))]
public class DbPlayerStoryState : DbPlayerData
{
    [Column("StoryType")]
    [Required]
    public StoryTypes StoryType { get; set; }

    [Column("StoryId")]
    [Required]
    public int StoryId { get; set; }

    [Column("State")]
    [Required]
    public StoryState State { get; set; }
}
