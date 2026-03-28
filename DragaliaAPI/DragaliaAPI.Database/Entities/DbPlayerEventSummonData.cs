using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(EventId))]
public class DbPlayerEventSummonData : DbPlayerData
{
    public int EventId { get; set; }

    public int Points { get; set; }

    public int BoxNumber { get; set; } = 1;

    public List<DbPlayerEventSummonItem> Items { get; set; } = [];

    private class Configuration : IEntityTypeConfiguration<DbPlayerEventSummonData>
    {
        public void Configure(EntityTypeBuilder<DbPlayerEventSummonData> builder)
        {
            builder
                .HasOne(e => e.Owner)
                .WithMany(e => e.EventSummonData)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Items)
                .WithOne(x => x.EventSummonData)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
