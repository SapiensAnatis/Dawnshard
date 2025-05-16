using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbPlayerHelperUseDate
{
    public long HelperViewerId { get; set; }

    public long PlayerViewerId { get; set; }

    public DateTimeOffset UseDate { get; set; }

    public DbPlayerHelper? Helper { get; set; }

    public DbPlayer? Player { get; set; }

    private class Configuration : IEntityTypeConfiguration<DbPlayerHelperUseDate>
    {
        public void Configure(EntityTypeBuilder<DbPlayerHelperUseDate> builder)
        {
            builder.HasKey(e => new { e.HelperViewerId, e.PlayerViewerId });

            builder.HasOne(e => e.Player).WithMany().OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.Helper)
                .WithMany(e => e.UseDates)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
