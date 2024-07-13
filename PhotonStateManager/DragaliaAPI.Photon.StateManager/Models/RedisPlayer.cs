using DragaliaAPI.Photon.Shared.Models;
using Redis.OM.Modeling;

namespace DragaliaAPI.Photon.StateManager.Models;

/// <summary>
/// Represents a player as stored in Redis.
/// </summary>
public class RedisPlayer
{
    public RedisPlayer(Player player)
    {
        this.ActorNr = player.ActorNr;
        this.ViewerId = player.ViewerId;
        this.PartyNoList = player.PartyNoList;
    }

    public RedisPlayer() { }

    /// <summary>
    /// Gets or sets the player's actor number.
    /// </summary>
    public int ActorNr { get; set; }

    /// <summary>
    /// Gets or sets the player's viewer ID.
    /// </summary>
    [Indexed]
    public long ViewerId { get; set; }

    public IEnumerable<int> PartyNoList { get; set; } = [];

    public Player ToPlayer() =>
        new()
        {
            ViewerId = this.ViewerId,
            ActorNr = this.ActorNr,
            PartyNoList = this.PartyNoList
        };
}
