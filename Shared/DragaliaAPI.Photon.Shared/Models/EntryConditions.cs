namespace DragaliaAPI.Photon.Shared.Models;

/// <summary>
/// Object representing room entry conditions.
/// </summary>
public class EntryConditions
{
    /// <summary>
    /// The list of disallowed elements for this room.
    /// </summary>
    public IEnumerable<int> UnacceptedElementTypeList { get; set; } = Array.Empty<int>();

    /// <summary>
    /// The list of disallowed weapons for this room.
    /// </summary>
    public IEnumerable<int> UnacceptedWeaponTypeList { get; set; } = Array.Empty<int>();

    /// <summary>
    /// The minimum required party might to join this room.
    /// </summary>
    public int RequiredPartyPower { get; set; }

    /// <summary>
    /// The room's objective ID.
    /// </summary>
    public int ObjectiveTextId { get; set; }
}
