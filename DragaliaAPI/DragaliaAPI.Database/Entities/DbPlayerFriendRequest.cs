using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbPlayerFriendRequest
{
    public long FromPlayerViewerId { get; set; }

    public long ToPlayerViewerId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the friend request is new from the perspective
    /// of the <see cref="ToPlayer"/>.
    /// </summary>
    public bool IsNew { get; set; }

    public DbPlayer? FromPlayer { get; set; }

    public DbPlayer? ToPlayer { get; set; }

    private class Configuration : IEntityTypeConfiguration<DbPlayerFriendRequest>
    {
        public void Configure(EntityTypeBuilder<DbPlayerFriendRequest> builder)
        {
            builder.HasKey(e => new { e.FromPlayerViewerId, e.ToPlayerViewerId });

            builder.HasOne(e => e.FromPlayer).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ToPlayer).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
