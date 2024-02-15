using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database;

[Index(nameof(QuestId))]
public class DbTimeAttackClear
{
    [Key]
    public required string GameId { get; set; }

    public int QuestId { get; set; }

    public float Time { get; set; }

    public List<DbTimeAttackPlayer> Players { get; set; } = new();

    /// <summary>
    /// Player hash.
    /// </summary>
    /// <remarks>
    /// Used to only allow 1 displayed leaderboard entry per set of players.
    /// </remarks>
    [NotMapped]
    public int PlayersHash =>
        string.Join(string.Empty, this.Players.Select(x => x.ViewerId).Order()).GetHashCode();
}
