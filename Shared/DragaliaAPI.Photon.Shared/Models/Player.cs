namespace DragaliaAPI.Photon.Shared.Models;

/// <summary>
/// Player data transfer object.
/// </summary>
public class Player
{
    /// <summary>
    /// The player's actor number.
    /// </summary>
    public int ActorNr { get; set; }

    /// <summary>
    /// The player's viewer ID.
    /// </summary>
    public long ViewerId { get; set; }

    /// <summary>
    /// The player's selected party slot(s).
    /// </summary>
    public IEnumerable<int> PartyNoList { get; set; } = Enumerable.Empty<int>();
}
