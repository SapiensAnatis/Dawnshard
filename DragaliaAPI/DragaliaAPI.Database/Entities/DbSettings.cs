using DragaliaAPI.Database.Entities.Owned;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbSettings
{
    public long ViewerId { get; set; }

    public required PlayerSettings SettingsJson { get; set; }

    public DbPlayer? Player { get; set; }

    private class Configuration : IEntityTypeConfiguration<DbSettings>
    {
        public void Configure(EntityTypeBuilder<DbSettings> builder)
        {
            builder.HasKey(e => new { e.ViewerId });

            builder.OwnsOne(e => e.SettingsJson).ToJson();

            builder
                .HasOne(e => e.Player)
                .WithOne(e => e.Settings)
                .HasForeignKey<DbSettings>(e => e.ViewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
