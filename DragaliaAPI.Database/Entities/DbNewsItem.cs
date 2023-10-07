using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

public class DbNewsItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public required string Headline { get; set; }

    public DateTimeOffset Time { get; set; }

    public required string Description { get; set; }
}
