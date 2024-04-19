using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

public class DbNewsItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [StringLength(256)]
    public required string Headline { get; set; }

    public DateTimeOffset Time { get; set; }

    [StringLength(4096)]
    public required string Description { get; set; }
}
