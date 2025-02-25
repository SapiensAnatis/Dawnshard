using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Represents a friendship that contains one or two players.
/// </summary>
public class DbPlayerFriendship
{
    public int FriendshipId { get; set; }

    /// <summary>
    /// Whether the friendship has been accepted.
    /// </summary>
    public bool IsAccepted { get; set; }

    public List<DbPlayer> Players { get; set; } = [];

    private class Configuration : IEntityTypeConfiguration<DbPlayerFriendship>
    {
        public void Configure(EntityTypeBuilder<DbPlayerFriendship> builder)
        {
            builder.HasKey(e => e.FriendshipId);

            builder
                .HasMany(e => e.Players)
                .WithMany(e => e.Friendships)
                .UsingEntity<DbPlayerFriendshipPlayer>(
                    l => l.HasOne(e => e.Player).WithMany().OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne(e => e.Friendship).WithMany().OnDelete(DeleteBehavior.Restrict)
                );
        }
    }
}
