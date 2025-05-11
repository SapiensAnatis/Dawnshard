namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Join table for the many-to-many relationship between <see cref="DbPlayer"/> and <see cref="DbPlayerFriendship"/>.
/// </summary>
public class DbPlayerFriendshipPlayer
{
    public long PlayerViewerId { get; set; }

    public int FriendshipId { get; set; }

    public bool IsNew { get; set; }

    public DbPlayer? Player { get; set; }

    public DbPlayerFriendship? Friendship { get; set; }
}
