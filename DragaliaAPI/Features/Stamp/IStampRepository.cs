using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Stamp;

public interface IStampRepository
{
    /// <summary>
    /// Gets a query of the equipped stamps for the current player.
    /// </summary>
    IQueryable<DbEquippedStamp> EquippedStamps { get; }

    /// <summary>
    /// Sets the equipped stamp list to a new stamp list.
    /// </summary>
    /// <param name="newStampList">The new stamp list.</param>
    /// <returns>The task.</returns>
    Task SetEquipStampList(IEnumerable<DbEquippedStamp> newStampList);
}
