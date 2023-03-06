using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerStoryState")]
[Index(nameof(DeviceAccountId))]
public class DbPlayerStoryState : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

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
