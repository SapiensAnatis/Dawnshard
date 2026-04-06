using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

[Table("PlayerGatherItem")]
[PrimaryKey(nameof(ViewerId), nameof(GatherItemId))]
public class DbPlayerGatherItem : DbPlayerData
{
    [Column("GatherItemId")]
    [Required]
    public int GatherItemId { get; set; }

    [Column("Quantity")]
    [Required]
    public int Quantity { get; set; }

    [Column("QuestTakeWeeklyQuantity")]
    [Required]
    public int QuestTakeWeeklyQuantity { get; set; }

    [Column("QuestLastWeeklyResetTime")]
    [Required]
    public DateTimeOffset QuestLastWeeklyResetTime { get; set; }

    private class Configuration : IEntityTypeConfiguration<DbPlayerGatherItem>
    {
        public void Configure(EntityTypeBuilder<DbPlayerGatherItem> builder)
        {
            builder
                .HasOne(e => e.Owner)
                .WithMany(e => e.GatherItemList)
                .HasForeignKey(e => e.ViewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
