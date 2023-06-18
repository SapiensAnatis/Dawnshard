using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Stamp;

public interface IStampService
{
    /// <summary>
    /// Get a list of stamps owned by the player.
    /// </summary>
    /// <returns>A list of stamps.</returns>
    Task<IEnumerable<StampList>> GetStampList();

    /// <summary>
    /// Update a player's equipped stamp list.
    /// </summary>
    /// <param name="newStampList">The new stamp list.</param>
    /// <returns>The new stamp list after validation.</returns>
    /// <remarks>
    /// Will currently return <paramref name="newStampList"/> verbatim until validation is added.
    /// </remarks>
    Task<IEnumerable<EquipStampList>> SetEquipStampList(IEnumerable<EquipStampList> newStampList);
}
